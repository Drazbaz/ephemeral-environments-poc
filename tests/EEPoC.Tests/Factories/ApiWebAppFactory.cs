using Api;
using Api.Infrastructure.Persistance;
using EEPoC.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EEPoC.Tests.Factories
{
    public sealed class ApiWebAppFactory(PostgreSqlFixture fixture) : WebApplicationFactory<Program>
    {
        private readonly string _connectionString = fixture.PostgreSqlContainer.GetConnectionString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<ApiDbContext>>();
                services.RemoveAll<ApiDbContext>();
                services.AddDbContext<ApiDbContext>(options => options.UseNpgsql(_connectionString));
            });
        }
    }
}
