using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(DirectoriesMenuService))]
public class DirectoriesMenuServiceShould
{
    [Fact]
    public void ReturnNonEmptyMenuItems()
    {
        var items = DirectoriesMenuService.GetDirectoriesMenuItems();

        items.ShouldNotBeNull();
        items.ShouldNotBeEmpty();
        items.Count.ShouldBe(3);
    }

    [Fact]
    public void ContainDirectoriesRootItem()
    {
        var items = DirectoriesMenuService.GetDirectoriesMenuItems();

        var root = items.FirstOrDefault(i => i.Id == "Directories");
        root.ShouldNotBeNull();
        root!.Id.ShouldBe("Directories");
        root.Text.ShouldBe("Directories");
        root.IconName.ShouldBe(IconName.WindowPlus);
        root.IconColor.ShouldBe(IconColor.Danger);
        root.Href.ShouldBeNull();
        root.ParentId.ShouldBeNull();
    }

    [Theory]
    [InlineData("Directories Rename", "/directories/rename", IconName.Folder,  IconColor.Primary, "Rename Directory(ies)")]
    [InlineData("10",                 "/directories/move",   IconName.FolderX, IconColor.Primary, "Move Directory(ies)")]
    public void ContainSpecificChildItem(string id, string href, IconName iconName, IconColor iconColor, string text)
    {
        var items = DirectoriesMenuService.GetDirectoriesMenuItems();

        var item = items.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Href.ShouldBe(href);
        item.IconName.ShouldBe(iconName);
        item.IconColor.ShouldBe(iconColor);
        item.Text.ShouldBe(text);
        item.ParentId.ShouldBe("Directories");
    }

    [Fact]
    public void AllChildrenShouldReferenceDirectoriesRoot()
    {
        var items = DirectoriesMenuService.GetDirectoriesMenuItems();

        var children = items.Where(i => i.Id != "Directories").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.ParentId == "Directories").ShouldBeTrue();
    }

    [Fact]
    public void AllItemsShouldHaveUniqueIds()
    {
        var items = DirectoriesMenuService.GetDirectoriesMenuItems();

        var uniqueIds = items.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Count.ShouldBe(items.Count);
    }

    [Fact]
    public void AllChildrenShouldUsePrimaryIconColor()
    {
        var items = DirectoriesMenuService.GetDirectoriesMenuItems();

        var children = items.Where(i => i.Id != "Directories").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.IconColor == IconColor.Primary).ShouldBeTrue();
    }
}
