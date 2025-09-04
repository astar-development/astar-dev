using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public static class GamesMenuService
{
    private const string KidsGamesRoot     = "Kids Games";
    private const string KidsGamesRootMenu = "Kids Games Root Menu";

    public static List<NavItem> GetGamesMenuItems() =>
    [
        new() { Id = KidsGamesRoot, IconName = IconName.Dice6, Text = KidsGamesRoot, IconColor = IconColor.Warning },
        new()
        {
            Id             = "List",
            Href           = "/kids-games",
            Text           = "List",
            CustomIconName = "fa-solid fa-puzzle-piece",
            Class          = "menuIcon",
            IconColor      = IconColor.Warning,
            ParentId       = KidsGamesRoot
        },
        new()
        {
            Id        = "Halving",
            Href      = "/kids-games/halving",
            IconName  = IconName.InputCursorText,
            Class     = "menuIcon",
            Text      = "Halving",
            IconColor = IconColor.Warning,
            ParentId  = KidsGamesRoot
        },
        new()
        {
            Id        = "Sharing",
            Href      = "/kids-games/matching",
            Class     = "menuIcon",
            IconName  = IconName.Amazon,
            IconColor = IconColor.Warning,
            Text      = "Matching",
            ParentId  = KidsGamesRoot
        }
    ];
}
