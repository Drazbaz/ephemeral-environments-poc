using Testcontainers.PostgreSql;

namespace Api.Tests.Integration
{
    public sealed class PostgreSqlContainerTests : IAsyncLifetime
    {
        public PostgreSqlContainer PostgreSqlContainer { get; } = new PostgreSqlBuilder("postgres:latest").Build();

        public Task InitializeAsync()
        {
            return PostgreSqlContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return PostgreSqlContainer.DisposeAsync().AsTask();
        }
    }
}
