using EEPoC.Tests.Fixtures;
using Microsoft.Playwright;

namespace EEPoC.Tests.E2E
{
    [Trait("Category", "E2E")]
    public sealed class UserBrowserTests(E2EFixture fixture) : IClassFixture<E2EFixture>, IAsyncLifetime
    {
        private readonly E2EFixture _fixture = fixture;
        private IPlaywright _playwright = null!;
        private IBrowser _browser = null!;
        private IPage _page = null!;

        public async Task InitializeAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.ConnectAsync(_fixture.PlaywrightContainer.GetConnectionString());

            _page = await _browser.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            await _page.CloseAsync();
            await _browser.CloseAsync();
            _playwright.Dispose();
        }

        [Fact]
        public async Task CreateUserPersistsAfterReload()
        {
            await _page.GotoAsync("http://web:80");
            await Assertions.Expect(_page.GetByRole(AriaRole.Heading, new() { Name = "Users", Exact = true })).ToBeVisibleAsync();

            var name = $"E2E User {Guid.NewGuid():N}";
            await _page.GetByLabel("Name", new() { Exact = true }).FillAsync(name);
            await _page.GetByRole(AriaRole.Button, new() { Name = "Add user", Exact = true }).ClickAsync();
            var user = _page.GetByRole(AriaRole.Listitem).Filter(new() { HasText = name });
            await Assertions.Expect(user).ToBeVisibleAsync();

            await _page.ReloadAsync();
            await Assertions.Expect(user).ToBeVisibleAsync();
            await Assertions.Expect(_page.GetByRole(AriaRole.Alert)).ToHaveCountAsync(0);
        }

        [Fact]
        public async Task BlankNameCannotBeSubmitted()
        {
            await _page.GotoAsync("http://web:80");
            var submit = _page.GetByRole(AriaRole.Button, new() { Name = "Add user", Exact = true });
            await Assertions.Expect(submit).ToBeDisabledAsync();

            await _page.GetByLabel("Name", new() { Exact = true }).FillAsync("   ");
            await Assertions.Expect(submit).ToBeDisabledAsync();
        }
    }
}