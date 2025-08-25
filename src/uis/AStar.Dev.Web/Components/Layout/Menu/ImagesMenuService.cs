using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public static class ImagesMenuService
{
    private const string ImagesRoot = "Images";

    public static List<NavItem> GetImagesMenuItems() =>
    [
        new() { Id = ImagesRoot, IconName = IconName.LayoutSidebarInset, Text = ImagesRoot, IconColor = IconColor.Primary },
        new()
        {
            Id        = "Random Image",
            Href      = "/images/random-image",
            IconName  = IconName.Image,
            Text      = "Random Image",
            ParentId  = ImagesRoot,
            IconColor = IconColor.Success,
            Level     = 2
        },
        new()
        {
            Id        = "Duplicate Images",
            Href      = "/images/duplicate-images",
            IconName  = IconName.Images,
            Text      = "Duplicate Images",
            ParentId  = ImagesRoot,
            IconColor = IconColor.Success,
            Level     = 3
        },
        new()
        {
            Id        = "Move Images",
            Href      = "/images/move-images",
            IconName  = IconName.FileTypeMov,
            Text      = "Move Images",
            ParentId  = ImagesRoot,
            IconColor = IconColor.Success,
            Level     = 3
        },
        new()
        {
            Id        = "Rename Images",
            Href      = "/images/rename-images",
            IconName  = IconName.FileImage,
            Text      = "Rename Images",
            ParentId  = ImagesRoot,
            IconColor = IconColor.Success,
            Level     = 1
        },
        new()
        {
            Id        = "Scrape Images",
            Href      = "/images/scrape-images",
            IconName  = IconName.Globe,
            IconColor = IconColor.Success,
            Text      = "Scrape Images",
            ParentId  = ImagesRoot
        }
    ];
}
