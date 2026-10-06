using Api;
using Api.Application.Commands;
using Api.Domain.Entities;
using EEPoC.Tests.Factories;
using EEPoC.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace EEPoC.Tests.Integration
{
    public sealed class UserEndpointTests : IClassFixture<PostgreSqlFixture>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _webAppFactory;
        private readonly HttpClient _httpClient;

        public UserEndpointTests(PostgreSqlFixture fixture)
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

            var createdUser = await response.Content.ReadFromJsonAsync<User>();
            Assert.NotNull(createdUser);
            Assert.NotEqual(Guid.Empty, createdUser.Id);
            Assert.Equal(user.Name, createdUser.Name);
            Assert.Equal(
                $"/api/user/{createdUser.Id}",
                response.Headers.Location?.ToString());
        }

        public void Dispose()
        {
            _webAppFactory.Dispose();
        }
    }
}
