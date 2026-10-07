using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.Playwright;
using Testcontainers.Playwright;
using Testcontainers.PostgreSql;

namespace EEPoC.Tests.Fixtures
{
    public sealed class E2EFixture : IAsyncLifetime
    {
        private static readonly string PlaywrightVersion =
            typeof(IPlaywright)
                .Assembly
                .GetName()
                .Version!
                .ToString(3);

        private INetwork _network = null!;
        private PostgreSqlContainer _postgresContainer = null!;
        private IContainer _serverContainer = null!;
        private IContainer _webContainer = null!;
        public required PlaywrightContainer PlaywrightContainer;

        public async Task InitializeAsync()
        {
            _network = new NetworkBuilder()
                .WithName($"E2E_{Guid.NewGuid():N}")
                .Build();
            await _network.CreateAsync();

            _postgresContainer = new PostgreSqlBuilder("postgres:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("db")
                .Build();
            await _postgresContainer.StartAsync();

            _serverContainer = new ContainerBuilder("ephemeral-environments-poc-server:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("server")
                .WithEnvironment("DB_USER", "postgres")
                .WithEnvironment("DB_PASSWORD", "postgres")
                .WithPortBinding(8080, true)
                .WithWaitStrategy(
                    Wait
                        .ForUnixContainer()
                        .UntilHttpRequestIsSucceeded(r => r.ForPort(8080).ForPath("/api/health")))
                .Build();
            await _serverContainer.StartAsync();

            _webContainer = new ContainerBuilder("ephemeral-environments-poc-web:latest")
                .WithNetwork(_network)
                .WithNetworkAliases("web")
                .WithEnvironment("API_BASE_URL", "http://server:8080")
                .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(80))
                .Build();
            await _webContainer.StartAsync();

            PlaywrightContainer = new PlaywrightBuilder($"mcr.microsoft.com/playwright:v{PlaywrightVersion}")
                .WithNetwork(_network)
                .Build();
            await PlaywrightContainer.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await PlaywrightContainer.DisposeAsync();
            await _webContainer.DisposeAsync();
            await _serverContainer.DisposeAsync();
            await _postgresContainer.DisposeAsync();
            await _network.DisposeAsync();
        }
    }
}