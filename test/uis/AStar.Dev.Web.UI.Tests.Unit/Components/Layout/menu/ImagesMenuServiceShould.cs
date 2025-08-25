using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(ImagesMenuService))]
public class ImagesMenuServiceShould
{
    [Fact]
    public void ReturnNonEmptyMenuItems()
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        items.ShouldNotBeNull();
        items.ShouldNotBeEmpty();
        items.Count.ShouldBe(6);
    }

    [Fact]
    public void ContainImagesRootItem()
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        var root = items.FirstOrDefault(i => i.Id == "Images");
        root.ShouldNotBeNull();
        root!.Id.ShouldBe("Images");
        root.Text.ShouldBe("Images");
        root.IconName.ShouldBe(IconName.LayoutSidebarInset);
        root.IconColor.ShouldBe(IconColor.Primary);
        root.Href.ShouldBeNull();
        root.ParentId.ShouldBeNull();
    }

    [Theory]
    [InlineData("Random Image",     "/images/random-image",     IconName.Image,       IconColor.Success, "Random Image")]
    [InlineData("Duplicate Images", "/images/duplicate-images", IconName.Images,      IconColor.Success, "Duplicate Images")]
    [InlineData("Move Images",      "/images/move-images",      IconName.FileTypeMov, IconColor.Success, "Move Images")]
    [InlineData("Rename Images",    "/images/rename-images",    IconName.FileImage,   IconColor.Success, "Rename Images")]
    [InlineData("Scrape Images",    "/images/scrape-images",    IconName.Globe,       IconColor.Success, "Scrape Images")]
    public void ContainSpecificChildItem(string id, string href, IconName iconName, IconColor iconColor, string text)
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        var item = items.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Href.ShouldBe(href);
        item.IconName.ShouldBe(iconName);
        item.IconColor.ShouldBe(iconColor);
        item.Text.ShouldBe(text);
        item.ParentId.ShouldBe("Images");
    }

    [Theory]
    [InlineData("Random Image",     2)]
    [InlineData("Duplicate Images", 3)]
    [InlineData("Move Images",      3)]
    [InlineData("Rename Images",    1)]
    public void ItemsWithExplicitLevelShouldHaveExpectedLevel(string id, int expectedLevel)
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        var item = items.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Level.ShouldBe(expectedLevel);
    }

    [Fact]
    public void AllChildrenShouldReferenceImagesRoot()
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        var children = items.Where(i => i.Id != "Images").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.ParentId == "Images").ShouldBeTrue();
    }

    [Fact]
    public void AllChildrenShouldUseSuccessIconColor()
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        var children = items.Where(i => i.Id != "Images").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.IconColor == IconColor.Success).ShouldBeTrue();
    }

    [Fact]
    public void AllItemsShouldHaveUniqueIds()
    {
        var items = ImagesMenuService.GetImagesMenuItems();

        var uniqueIds = items.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Count.ShouldBe(items.Count);
    }
}
