using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(NuGetMenuService))]
public class NuGetMenuServiceShould
{
    [Fact]
    public void ReturnNonEmptyMenuItems()
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        menuItems.ShouldNotBeNull();
        menuItems.ShouldNotBeEmpty();
        menuItems.Count.ShouldBe(3);
    }

    [Fact]
    public void ContainNuGetRootItem()
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        var root = menuItems.FirstOrDefault(i => i.Id == "NuGet Documentation");
        root.ShouldNotBeNull();
        root.Id.ShouldBe("NuGet Documentation");
        root.Text.ShouldBe("NuGet Documentation");
        root.IconName.ShouldBe(IconName.WindowPlus);
        root.IconColor.ShouldBe(IconColor.Danger);
        root.ParentId.ShouldBeNull();
        root.Href.ShouldBeNull(); // root item does not navigate
    }

    [Fact]
    public void ContainNuGetRootMenu()
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        var root = menuItems.FirstOrDefault(i => i.Id == "NuGet Documentation Root Menu");
        root.ShouldNotBeNull();
        root.Text.ShouldBe("Root Docs");
        root.IconName.ShouldBe(IconName.InputCursorText);
        root.IconColor.ShouldBe(IconColor.Danger);
        root.ParentId.ShouldBe("NuGet Documentation");
        root.Href.ShouldBe("/nuget-documentation");
    }

    [Theory]
    [InlineData("functional-extensions", "/nuget-documentation/astar-dev-functional-extensions", IconName.Book, "Functional Extensions")]
    public void ContainSpecificChildItem(string id, string href, IconName iconName, string text)
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        var item = menuItems.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Href.ShouldBe(href);
        item.IconName.ShouldBe(iconName);
        item.Text.ShouldBe(text);
        item.IconColor.ShouldBe(IconColor.Danger);
        item.ParentId.ShouldBe("NuGet Documentation Root Menu");
    }

    [Fact]
    public void AllItemsShouldHaveUniqueIds()
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        var uniqueIds = menuItems.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Count.ShouldBe(menuItems.Count);
    }

    [Fact]
    public void AllChildrenShouldReferenceRootParent()
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        var children = menuItems.Where(i => i.Id != "NuGet Documentation").ToList();
        children.ShouldNotBeEmpty();
        children.Any(c => c.ParentId == "NuGet Documentation").ShouldBeTrue();
    }

    [Fact]
    public void AllItemsShouldUseDangerIconColor()
    {
        var menuItems = NuGetMenuService.GetNugetDocumentationItems();

        menuItems.All(i => i.IconColor == IconColor.Danger).ShouldBeTrue();
    }
}
