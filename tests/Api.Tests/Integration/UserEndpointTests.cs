using Api.Application.Commands;
using Api.Tests.Factories;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace Api.Tests.Integration
{
    public sealed class UserEndpointTests : IClassFixture<PostgreSqlContainerTests>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _webAppFactory;
        private readonly HttpClient _httpClient;

        public UserEndpointTests(PostgreSqlContainerTests fixture)
        {
            _webAppFactory = new ApiWebAppFactory(fixture);
            _httpClient = _webAppFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task CreateUser()
        {
            var user = new CreateUserCommand("Bob");
            var response = await _httpClient.PostAsJsonAsync("/api/user", user);
            Assert.True(response.IsSuccessStatusCode);
        }

        public void Dispose()
        {
            _webAppFactory.Dispose();
        }
    }
}
