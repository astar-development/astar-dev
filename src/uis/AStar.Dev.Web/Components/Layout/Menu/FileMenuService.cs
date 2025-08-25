using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public static class FileMenuService
{
    private const string FilesRoot = "Files";

    public static List<NavItem> GetFileMenuItems() =>
    [
        new() { Id = FilesRoot, IconName = IconName.LayoutSidebarInset, Text = FilesRoot, IconColor = IconColor.Primary },
        new()
        {
            Id        = "Duplicate Files",
            Href      = "/images/duplicate-files",
            IconName  = IconName.Files,
            IconColor = IconColor.Warning,
            Text      = "Duplicate Files",
            ParentId  = FilesRoot
        },
        new()
        {
            Id        = "Move Files",
            Href      = "/images/move-files",
            IconName  = IconName.FileTypeMov,
            IconColor = IconColor.Warning,
            Text      = "Move Files",
            ParentId  = FilesRoot
        },
        new()
        {
            Id        = "Rename Files",
            Href      = "/images/rename-files",
            IconName  = IconName.FileImage,
            IconColor = IconColor.Warning,
            Text      = "Rename Files",
            ParentId  = FilesRoot
        }
    ];
}
