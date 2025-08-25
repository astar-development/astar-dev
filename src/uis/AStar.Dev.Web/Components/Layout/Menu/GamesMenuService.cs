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
            Id        = KidsGamesRootMenu,
            Href      = "/kids-games",
            IconName  = IconName.InputCursorText,
            Text      = "Games List",
            IconColor = IconColor.Warning,
            ParentId  = KidsGamesRoot
        },
        new()
        {
            Id        = "List",
            Href      = "/kids-games",
            IconName  = IconName.InputCursorText,
            Text      = "List",
            IconColor = IconColor.Warning,
            ParentId  = KidsGamesRootMenu
        },
        new()
        {
            Id        = "Halving",
            Href      = "/kids-games/halving",
            IconName  = IconName.InputCursorText,
            Text      = "Halving",
            IconColor = IconColor.Warning,
            ParentId  = KidsGamesRootMenu
        },
        new()
        {
            Id        = "Sharing",
            Href      = "/kids-games/matching",
            IconName  = IconName.Amazon,
            IconColor = IconColor.Warning,
            Text      = "Matching",
            ParentId  = KidsGamesRootMenu
        }
    ];
}
