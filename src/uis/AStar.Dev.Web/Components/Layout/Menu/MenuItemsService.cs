using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

/// <inheritdoc />
public class MenuItemsService : IMenuItemsService
{
    /// <inheritdoc />
    public IEnumerable<NavItem> GetNavItems() => BuildMenuItems().ToList();

    private static List<NavItem> BuildMenuItems() =>
        ImagesMenuService.GetImagesMenuItems()
                         .Concat(FileMenuService.GetFileMenuItems())
                         .Concat(DirectoriesMenuService.GetDirectoriesMenuItems())
                         .Concat(NuGetMenuService.GetNugetDocumentationItems())
                         .Concat(GamesMenuService.GetGamesMenuItems())
                         .Concat(AdminMenuService.GetAdminMenuItems())
                         .ToList();
}
