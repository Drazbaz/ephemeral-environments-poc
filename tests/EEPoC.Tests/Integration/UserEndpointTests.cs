using Api;
using Api.Application.Commands;
using Api.Domain.Entities;
using EEPoC.Tests.Factories;
using EEPoC.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;

namespace EEPoC.Tests.Integration
{
    [Trait("Category", "Integration")]
    public sealed class UserEndpointTests : IClassFixture<PostgreSqlFixture>, IDisposable
    {
        private readonly WebApplicationFactory<Program> _apiFactory;
        private readonly HttpClient _httpClient;

        public UserEndpointTests(PostgreSqlFixture fixture)
        {
            _apiFactory = new ApiFactory(fixture);
            _httpClient = _apiFactory.CreateClient(new WebApplicationFactoryClientOptions
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
            _apiFactory.Dispose();
        }
    }
}
