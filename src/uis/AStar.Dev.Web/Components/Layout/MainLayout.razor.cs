using AStar.Dev.Web.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AStar.Dev.Web.Components.Layout;

public partial class MainLayout
{
    private IEnumerable<NavItem>? _navItems;
    private Sidebar               _sidebar = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private ILogger<MainLayout> Logger { get; set; } = null!;

    [Inject]
    private MenuItemsService MenuItemsService { get; set; } = null!;

    private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
    {
        _navItems ??= MenuItemsService.GetNavItems();

        return await Task.FromResult(request.ApplyTo(_navItems));
    }

    private void ToggleSidebar() => _sidebar.ToggleSidebar();
}
