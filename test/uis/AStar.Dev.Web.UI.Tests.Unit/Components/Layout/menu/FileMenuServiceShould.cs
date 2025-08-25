using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(FileMenuService))]
public class FileMenuServiceShould
{
    [Fact]
    public void ReturnNonEmptyMenuItems()
    {
        var items = FileMenuService.GetFileMenuItems();

        items.ShouldNotBeNull();
        items.ShouldNotBeEmpty();
        items.Count.ShouldBe(4);
    }

    [Fact]
    public void ContainFilesRootItem()
    {
        var items = FileMenuService.GetFileMenuItems();

        var root = items.FirstOrDefault(i => i.Id == "Files");
        root.ShouldNotBeNull();
        root!.Id.ShouldBe("Files");
        root.Text.ShouldBe("Files");
        root.IconName.ShouldBe(IconName.LayoutSidebarInset);
        root.IconColor.ShouldBe(IconColor.Primary);
        root.Href.ShouldBeNull();
        root.ParentId.ShouldBeNull();
    }

    [Theory]
    [InlineData("Duplicate Files", "/images/duplicate-files", IconName.Files,       IconColor.Warning, "Duplicate Files")]
    [InlineData("Move Files",      "/images/move-files",      IconName.FileTypeMov, IconColor.Warning, "Move Files")]
    [InlineData("Rename Files",    "/images/rename-files",    IconName.FileImage,   IconColor.Warning, "Rename Files")]
    public void ContainSpecificChildItem(string id, string href, IconName iconName, IconColor iconColor, string text)
    {
        var items = FileMenuService.GetFileMenuItems();

        var item = items.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Href.ShouldBe(href);
        item.IconName.ShouldBe(iconName);
        item.IconColor.ShouldBe(iconColor);
        item.Text.ShouldBe(text);
        item.ParentId.ShouldBe("Files");
    }

    [Fact]
    public void AllChildrenShouldReferenceFilesRoot()
    {
        var items = FileMenuService.GetFileMenuItems();

        var children = items.Where(i => i.Id != "Files").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.ParentId == "Files").ShouldBeTrue();
    }

    [Fact]
    public void AllItemsShouldHaveUniqueIds()
    {
        var items = FileMenuService.GetFileMenuItems();

        var uniqueIds = items.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Count.ShouldBe(items.Count);
    }

    [Fact]
    public void AllChildrenShouldUseWarningIconColor()
    {
        var items = FileMenuService.GetFileMenuItems();

        var children = items.Where(i => i.Id != "Files").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.IconColor == IconColor.Warning).ShouldBeTrue();
    }
}
