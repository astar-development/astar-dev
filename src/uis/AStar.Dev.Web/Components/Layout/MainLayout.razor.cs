using AStar.Dev.Web.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Layout;

public partial class MainLayout
{
    private IEnumerable<NavItem>? navItems;
    private Sidebar               sidebar = null!;

    [Inject]
    private MenuItemsService MenuItemsService { get; set; } = null!;

    private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
    {
        navItems ??= MenuItemsService.GetNavItems();

        return await Task.FromResult(request.ApplyTo(navItems));
    }

    private void ToggleSidebar() => sidebar.ToggleSidebar();
}
