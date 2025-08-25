using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(MenuItemsService))]
public class MenuItemsServiceShould
{
    [Fact]
    public void ReturnNonEmptyNavItems()
    {
        // Arrange
        var service = new MenuItemsService();

        // Act
        var items = service.GetNavItems();

        // Assert
        items.ShouldNotBeNull();
        items.ShouldNotBeEmpty();
    }

    [Fact]
    public void IncludeAllFileMenuItems()
    {
        // Arrange
        var service  = new MenuItemsService();
        var expected = FileMenuService.GetFileMenuItems();

        // Act
        var all = service.GetNavItems().ToList();

        // Assert
        foreach(var exp in expected)
        {
            var actual = all.FirstOrDefault(i => i.Id == exp.Id);
            actual.ShouldNotBeNull($"Menu should contain File menu item with Id '{exp.Id}'");
            actual!.Text.ShouldBe(exp.Text);
            actual.IconName.ShouldBe(exp.IconName);
            actual.IconColor.ShouldBe(exp.IconColor);
            actual.Href.ShouldBe(exp.Href);
            actual.ParentId.ShouldBe(exp.ParentId);
        }
    }

    [Fact]
    public void IncludeAllNuGetMenuItems()
    {
        // Arrange
        var service  = new MenuItemsService();
        var expected = NuGetMenuService.GetNugetDocumentationItems();

        // Act
        var all = service.GetNavItems().ToList();

        // Assert
        foreach(var exp in expected)
        {
            var actual = all.FirstOrDefault(i => i.Id == exp.Id);
            actual.ShouldNotBeNull($"Menu should contain NuGet item with Id '{exp.Id}'");
            actual!.Text.ShouldBe(exp.Text);
            actual.IconName.ShouldBe(exp.IconName);
            actual.IconColor.ShouldBe(exp.IconColor);
            actual.Href.ShouldBe(exp.Href);
            actual.ParentId.ShouldBe(exp.ParentId);
        }
    }

    [Fact]
    public void ContainKnownRootItemsFromSources()
    {
        // Arrange
        var service = new MenuItemsService();

        // Act
        var all = service.GetNavItems().ToList();

        // Assert
        var filesRoot = all.FirstOrDefault(i => i.Id == "Files");
        filesRoot.ShouldNotBeNull();
        filesRoot!.Text.ShouldBe("Files");
        filesRoot.IconName.ShouldBe(IconName.LayoutSidebarInset);
        filesRoot.IconColor.ShouldBe(IconColor.Primary);
        filesRoot.Href.ShouldBeNull();
        filesRoot.ParentId.ShouldBeNull();

        var nugetRoot = all.FirstOrDefault(i => i.Id == "NuGet Documentation");
        nugetRoot.ShouldNotBeNull();
        nugetRoot!.Text.ShouldBe("NuGet Documentation");
        nugetRoot.IconName.ShouldBe(IconName.WindowPlus);
        nugetRoot.IconColor.ShouldBe(IconColor.Danger);
        nugetRoot.Href.ShouldBeNull();
        nugetRoot.ParentId.ShouldBeNull();
    }
}
