using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(GamesMenuService))]
public class GamesMenuServiceShould
{
    [Fact]
    public void ReturnNonEmptyMenuItems()
    {
        var items = GamesMenuService.GetGamesMenuItems();

        items.ShouldNotBeNull();
        items.ShouldNotBeEmpty();
        items.Count.ShouldBe(5);
    }

    [Fact]
    public void ContainKidsGamesRootItem()
    {
        var items = GamesMenuService.GetGamesMenuItems();

        var root = items.FirstOrDefault(i => i.Id == "Kids Games");
        root.ShouldNotBeNull();
        root!.Id.ShouldBe("Kids Games");
        root.Text.ShouldBe("Kids Games");
        root.IconName.ShouldBe(IconName.Dice6);
        root.IconColor.ShouldBe(IconColor.Warning);
        root.Href.ShouldBeNull();
        root.ParentId.ShouldBeNull();
    }

    [Theory]
    [InlineData("Halving", "/kids-games/halving",  IconName.InputCursorText, IconColor.Warning, "Halving")]
    [InlineData("Sharing", "/kids-games/matching", IconName.Amazon,          IconColor.Warning, "Matching")]
    public void ContainSpecificChildItem(string id, string href, IconName iconName, IconColor iconColor, string text)
    {
        var items = GamesMenuService.GetGamesMenuItems();

        var item = items.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Href.ShouldBe(href);
        item.IconName.ShouldBe(iconName);
        item.IconColor.ShouldBe(iconColor);
        item.Text.ShouldBe(text);
        item.ParentId.ShouldBe("Kids Games Root Menu");
    }

    [Fact]
    public void AllChildrenShouldReferenceKidsGamesRoot()
    {
        var items = GamesMenuService.GetGamesMenuItems();

        var children = items.Where(i => i.Id != "Kids Games").ToList();
        children.ShouldNotBeEmpty();
        children.Count(c => c.ParentId == "Kids Games Root Menu").ShouldBe(children.Count - 1);
    }

    [Fact]
    public void AllItemsShouldHaveUniqueIds()
    {
        var items = GamesMenuService.GetGamesMenuItems();

        var uniqueIds = items.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Count.ShouldBe(items.Count);
    }

    [Fact]
    public void AllChildrenShouldUseWarningIconColor()
    {
        var items = GamesMenuService.GetGamesMenuItems();

        var children = items.Where(i => i.Id != "Kids Games").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.IconColor == IconColor.Warning).ShouldBeTrue();
    }
}
