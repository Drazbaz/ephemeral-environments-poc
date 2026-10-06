using Testcontainers.PostgreSql;

namespace EEPoC.Tests.Fixtures
{
    public sealed class PostgreSqlFixture : IAsyncLifetime
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
