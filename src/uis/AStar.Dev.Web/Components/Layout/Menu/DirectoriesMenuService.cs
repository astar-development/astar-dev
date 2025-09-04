using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public static class DirectoriesMenuService
{
    private const string DirectoriesRoot = "Directories";

    public static List<NavItem> GetDirectoriesMenuItems() =>
    [
        new() { Id = DirectoriesRoot, IconName = IconName.WindowPlus, Text = DirectoriesRoot, IconColor = IconColor.Danger },
        new()
        {
            Id        = "Directories Rename",
            Href      = "/directories/rename-directories",
            Class     = "menuIcon",
            IconName  = IconName.Folder,
            IconColor = IconColor.Primary,
            Text      = "Rename Directory(ies)",
            ParentId  = DirectoriesRoot
        },
        new()
        {
            Id        = "10",
            Href      = "/directories/move-directories",
            Class     = "menuIcon",
            IconName  = IconName.FolderX,
            IconColor = IconColor.Primary,
            Text      = "Move Directory(ies)",
            ParentId  = DirectoriesRoot
        }
    ];
}
