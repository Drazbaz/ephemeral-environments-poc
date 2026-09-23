using Testcontainers.PostgreSql;

namespace Api.Tests.Integration
{
    public sealed class PostgreSqlContainerTests : IAsyncLifetime
    {
        public PostgreSqlContainer PostgresContainer { get; } = new PostgreSqlBuilder("postgres:latest").Build();

        public Task InitializeAsync()
        {
            return PostgresContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return PostgresContainer.DisposeAsync().AsTask();
        }
    }
}
