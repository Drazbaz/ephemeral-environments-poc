using System.Collections.Concurrent;
using System.Diagnostics;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using EEPoC.Tests.Factories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;

namespace EEPoC.Tests.Fixtures
{
    public sealed class E2EFixture : IAsyncLifetime
    {
        private readonly PostgreSqlFixture _database = new();
        private readonly IContainer _browserContainer = new ContainerBuilder("mcr.microsoft.com/playwright:v1.58.0-noble")
            .WithEntrypoint("npx")
            .WithCommand("-y", "playwright@1.58.0", "run-server", "--port", "3000", "--host", "0.0.0.0")
            .WithPortBinding(3000, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(3000))
            .Build();
        private ApiWebAppFactory? _factory;
        private Process? _webServer;
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private string _webUrl = string.Empty;

        public async Task InitializeAsync()
        {
            try
            {
                await _database.InitializeAsync();
                _factory = new ApiWebAppFactory(_database);
                _factory.UseKestrel(0);
                _factory.StartServer();

                var server = _factory.Services.GetRequiredService<IServer>();
                var apiUrl = server.Features.Get<IServerAddressesFeature>()!.Addresses.Single();
                var environment = _factory.Services.GetRequiredService<IWebHostEnvironment>();
                var webDirectory = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "..", "web"));
                _webUrl = await StartWebServerAsync(webDirectory, apiUrl);

                await _browserContainer.StartAsync();
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.ConnectAsync(
                    $"ws://{_browserContainer.Hostname}:{_browserContainer.GetMappedPublicPort(3000)}/",
                    new() { ExposeNetwork = "<loopback>" });
            }
            catch
            {
                await DisposeAsync();
                throw;
            }
        }

        public Task<IBrowserContext> CreateContextAsync()
        {
            if (_browser is null) throw new InvalidOperationException("The browser fixture has not started.");
            return _browser.NewContextAsync(new() { BaseURL = _webUrl });
        }

        private async Task<string> StartWebServerAsync(string webDirectory, string apiUrl)
        {
            var ready = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var logs = new ConcurrentQueue<string>();
            var webServer = new Process
            {
                StartInfo = new ProcessStartInfo("node")
                {
                    WorkingDirectory = webDirectory,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };
            webServer.StartInfo.ArgumentList.Add("--input-type=module");
            webServer.StartInfo.ArgumentList.Add("--eval");
            webServer.StartInfo.ArgumentList.Add("""
                import { createServer } from 'vite';
                const server = await createServer({
                    server: {
                        host: '127.0.0.1',
                        port: 0,
                        proxy: { '/api': process.env.E2E_API_URL }
                    }
                });
                await server.listen();
                console.log('E2E_WEB_URL=' + server.resolvedUrls.local[0]);
                """);
            webServer.StartInfo.Environment["E2E_API_URL"] = apiUrl;
            webServer.OutputDataReceived += (_, args) =>
            {
                if (args.Data is not { } line) return;
                logs.Enqueue(line);
                if (line.StartsWith("E2E_WEB_URL=", StringComparison.Ordinal))
                {
                    ready.TrySetResult(line["E2E_WEB_URL=".Length..]);
                }
            };
            webServer.ErrorDataReceived += (_, args) =>
            {
                if (args.Data is { } line) logs.Enqueue(line);
            };
            webServer.Exited += (_, _) => ready.TrySetException(
                new InvalidOperationException($"Vite exited with code {webServer.ExitCode}. {string.Join(Environment.NewLine, logs)}"));

            try
            {
                webServer.Start();
            }
            catch
            {
                webServer.Dispose();
                throw;
            }

            _webServer = webServer;
            webServer.BeginOutputReadLine();
            webServer.BeginErrorReadLine();
            try
            {
                return await ready.Task.WaitAsync(TimeSpan.FromSeconds(30));
            }
            catch (TimeoutException exception)
            {
                throw new TimeoutException($"Vite did not start. {string.Join(Environment.NewLine, logs)}", exception);
            }
        }

        public async Task DisposeAsync()
        {
            try
            {
                if (_browser is not null) await _browser.DisposeAsync();
            }
            finally
            {
                _playwright?.Dispose();
                try
                {
                    await _browserContainer.DisposeAsync();
                }
                finally
                {
                    try
                    {
                        if (_webServer is not null)
                        {
                            using var webServer = _webServer;
                            _webServer = null;
                            if (!webServer.HasExited) webServer.Kill(entireProcessTree: true);
                            await webServer.WaitForExitAsync();
                        }
                    }
                    finally
                    {
                        try
                        {
                            if (_factory is not null) await _factory.DisposeAsync();
                        }
                        finally
                        {
                            await _database.DisposeAsync();
                        }
                    }
                }
            }
        }
    }
}