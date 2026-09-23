using Api.Infrastructure.Persistance;
using Api.Tests.Integration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Api.Tests.Factories
{
    public sealed class ApiWebAppFactory(PostgreSqlContainerTests fixture) : WebApplicationFactory<Program>
    {
        private readonly string _connectionString = fixture.PostgresContainer.GetConnectionString();

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
