using AStar.Dev.Web.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace AStar.Dev.Web.Components.Layout;

public partial class MainLayout
{
    private IEnumerable<NavItem>? _navItems;
    private Sidebar               _sidebar      = null!;
    private string                _defaultTheme = "dark";

    [Inject]
    private MenuItemsService MenuItemsService { get; set; } = null!;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var savedTheme = await JsRuntime.InvokeAsync<string>("localStorage.getItem", "theme");
        _defaultTheme = !string.IsNullOrEmpty(savedTheme) ? savedTheme : "dark";

        await base.OnInitializedAsync();
    }

    private async Task<SidebarDataProviderResult> SidebarDataProvider(SidebarDataProviderRequest request)
    {
        _navItems ??= MenuItemsService.GetNavItems();

        return await Task.FromResult(request.ApplyTo(_navItems));
    }

    private void ToggleSidebar() => _sidebar.ToggleSidebar();
}
