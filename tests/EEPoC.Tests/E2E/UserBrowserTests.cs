using EEPoC.Tests.Fixtures;
using Microsoft.Playwright;

namespace EEPoC.Tests.E2E
{
    [Trait("Category", "E2E")]
    public sealed class UserBrowserTests(E2EFixture fixture) : IClassFixture<E2EFixture>, IAsyncLifetime
    {
        private IBrowserContext? _context;
        private IPage _page = null!;

        public async Task InitializeAsync()
        {
            _context = await fixture.CreateContextAsync();
            _page = await _context.NewPageAsync();
        }

        public async Task DisposeAsync()
        {
            if (_context is not null) await _context.DisposeAsync();
        }

        [Fact]
        public async Task CreateUserPersistsAfterReload()
        {
            await _page.GotoAsync("/");
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
            await _page.GotoAsync("/");
            var submit = _page.GetByRole(AriaRole.Button, new() { Name = "Add user", Exact = true });
            await Assertions.Expect(submit).ToBeDisabledAsync();

            await _page.GetByLabel("Name", new() { Exact = true }).FillAsync("   ");
            await Assertions.Expect(submit).ToBeDisabledAsync();
        }
    }
}