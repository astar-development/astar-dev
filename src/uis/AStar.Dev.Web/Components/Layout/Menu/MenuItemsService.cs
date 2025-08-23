using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public class MenuItemsService
{
    private const string ImagesRoot             = "Images";
    private const string FilesRoot              = "Files";
    private const string DirectoriesRoot        = "Directories";
    private const string NuGetDocumentationRoot = "NuGet Documentation";
    private const string KidsGamesRoot          = "Kids Games";
    private const string AdminRoot              = "Admin";

    public IEnumerable<NavItem> GetNavItems() => BuildMenuItems();

    private List<NavItem> BuildMenuItems() =>
        GetImagesMenuItems()
            .Concat(GetFileMenuItems())
            .Concat(GetDirectories())
            .Concat(GetNugetDocumentation())
            .Concat(GetGamesMenuItems())
            .Concat(GetAdminMenuItems())
            .ToList();

    private List<NavItem> GetNugetDocumentation() =>
    [
        new() { Id = NuGetDocumentationRoot, IconName = IconName.WindowPlus, Text = NuGetDocumentationRoot, IconColor = IconColor.Danger },
        new()
        {
            Id       = "9",
            Href     = "/nuget-documentation",
            IconName = IconName.InputCursorText,
            Text     = "Root Docs",
            ParentId = NuGetDocumentationRoot
        },
        new()
        {
            Id       = "10",
            Href     = "/nuget-documentation/astar-dev-functional-extensions",
            IconName = IconName.FuelPump,
            Text     = "Functional Extensions",
            ParentId = NuGetDocumentationRoot
        }
    ];

    private List<NavItem> GetDirectories() =>
    [
        new() { Id = DirectoriesRoot, IconName = IconName.WindowPlus, Text = DirectoriesRoot, IconColor = IconColor.Danger },
        new()
        {
            Id       = "Directories Rename",
            Href     = "/directories/rename",
            IconName = IconName.FuelPump,
            Text     = "Rename Directory(ies)",
            ParentId = DirectoriesRoot
        },
        new()
        {
            Id       = "10",
            Href     = "/directories/move",
            IconName = IconName.FuelPump,
            Text     = "Move Directory(ies)",
            ParentId = DirectoriesRoot
        }
    ];

    private List<NavItem> GetGamesMenuItems() =>
    [
        new()
        {
            Id        = KidsGamesRoot,
            Href      = "/kids-games",
            IconName  = IconName.Dice6,
            IconColor = IconColor.Warning,
            Text      = KidsGamesRoot
        }
    ];

    private List<NavItem> GetImagesMenuItems() =>
    [
        new() { Id = ImagesRoot, IconName = IconName.LayoutSidebarInset, Text = ImagesRoot, IconColor = IconColor.Primary },
        new()
        {
            Id       = "Random Image",
            Href     = "/images/random-image",
            IconName = IconName.PersonSquare,
            Text     = "Random Image",
            ParentId = ImagesRoot,
            Level    = 2
        },
        new()
        {
            Id       = "Duplicate Images",
            Href     = "/images/duplicate-images",
            IconName = IconName.PersonSquare,
            Text     = "Duplicate Images",
            ParentId = ImagesRoot,
            Level    = 3
        },
        new()
        {
            Id       = "Move Images",
            Href     = "/images/move-images",
            IconName = IconName.PersonSquare,
            Text     = "Move Images",
            ParentId = ImagesRoot,
            Level    = 3
        },
        new()
        {
            Id       = "Rename Images",
            Href     = "/images/rename-images",
            IconName = IconName.PersonSquare,
            Text     = "Rename Images",
            ParentId = ImagesRoot,
            Level    = 1
        },
        new()
        {
            Id       = "Scrape Images",
            Href     = "/images/scrape-images",
            IconName = IconName.PersonSquare,
            Text     = "Scrape Images",
            ParentId = ImagesRoot
        }
    ];

    private List<NavItem> GetFileMenuItems() =>
    [
        new() { Id = FilesRoot, IconName = IconName.LayoutSidebarInset, Text = FilesRoot, IconColor = IconColor.Primary },
        new()
        {
            Id       = "Duplicate Files",
            Href     = "/images/duplicate-files",
            IconName = IconName.PersonSquare,
            Text     = "Duplicate Files",
            ParentId = FilesRoot
        },
        new()
        {
            Id       = "Move Files",
            Href     = "/images/move-files",
            IconName = IconName.PersonSquare,
            Text     = "Move Files",
            ParentId = FilesRoot
        },
        new()
        {
            Id       = "Rename Files",
            Href     = "/images/rename-files",
            IconName = IconName.PersonSquare,
            Text     = "Rename Files",
            ParentId = FilesRoot
        }
    ];

    private List<NavItem> GetAdminMenuItems() =>
    [
        new() { Id = AdminRoot, IconName = IconName.LayoutSidebarInset, Text = AdminRoot, IconColor = IconColor.Primary },
        new()
        {
            Id       = "Authentication Check",
            Href     = "/admin/authentication-check",
            IconName = IconName.PersonSquare,
            Text     = "Authentication Check",
            ParentId = AdminRoot
        },
        new()
        {
            Id       = "Site Config",
            Href     = "/images/site-config",
            IconName = IconName.PersonSquare,
            Text     = "Site Config",
            ParentId = AdminRoot
        },
        new()
        {
            Id       = "Scrape Directories",
            Href     = "/images/scrape-directories",
            IconName = IconName.PersonSquare,
            Text     = "Scrape Directories",
            ParentId = AdminRoot
        },
        new()
        {
            Id       = "Models to Ignore",
            Href     = "/images/Models to Ignore",
            IconName = IconName.PersonSquare,
            Text     = "Models to Ignore",
            ParentId = AdminRoot
        },
        new()
        {
            Id       = "Tags to Ignore",
            Href     = "/images/tags to Ignore",
            IconName = IconName.PersonSquare,
            Text     = "Tags to Ignore",
            ParentId = AdminRoot
        },
        new()
        {
            Id       = "Add files to database",
            Href     = "/images/Add files to database",
            IconName = IconName.PersonSquare,
            Text     = "Add files to database",
            ParentId = AdminRoot
        },
        new()
        {
            Id       = "API Usage",
            Href     = "/images/API Usage",
            IconName = IconName.PersonSquare,
            Text     = "API Usage",
            ParentId = AdminRoot
        }
    ];
}
