using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public static class NuGetMenuService
{
    private const string NuGetDocumentationRoot     = "NuGet Documentation";
    private const string NuGetDocumentationRootMenu = "NuGet Documentation Root Menu";

    public static List<NavItem> GetNugetDocumentationItems() =>
    [
        new() { Id = NuGetDocumentationRoot, IconName = IconName.WindowPlus, Text = NuGetDocumentationRoot, IconColor = IconColor.Danger },
        new()
        {
            Id        = NuGetDocumentationRootMenu,
            Href      = "/nuget-documentation",
            IconName  = IconName.InputCursorText,
            Class     = "menuIcon",
            Text      = "Root Docs",
            IconColor = IconColor.Danger,
            ParentId  = NuGetDocumentationRoot
        },
        new()
        {
            Id        = "functional-extensions",
            Href      = "/nuget-documentation/astar-dev-functional-extensions",
            IconName  = IconName.Book,
            IconColor = IconColor.Danger,
            Class     = "menuIcon",
            Text      = "Functional Extensions",
            ParentId  = NuGetDocumentationRootMenu
        }
    ];
}
