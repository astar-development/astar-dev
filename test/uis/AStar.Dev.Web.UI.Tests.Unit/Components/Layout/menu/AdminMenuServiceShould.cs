using AStar.Dev.Web.Components.Layout.Menu;
using BlazorBootstrap;

namespace AStar.Dev.Web.UI.Components.Layout.Menu;

[TestSubject(typeof(AdminMenuService))]
public class AdminMenuServiceShould
{
    [Fact]
    public void ReturnNonEmptyMenuItems()
    {
        var items = AdminMenuService.GetAdminMenuItems();

        items.ShouldNotBeNull();
        items.ShouldNotBeEmpty();
    }

    [Fact]
    public void ContainAdminRootItem()
    {
        var items = AdminMenuService.GetAdminMenuItems();

        var root = items.FirstOrDefault(i => i.Id == "Admin");
        root.ShouldNotBeNull();
        root!.Id.ShouldBe("Admin");
        root.Text.ShouldBe("Admin");
        root.IconName.ShouldBe(IconName.LayoutSidebarInset);
        root.IconColor.ShouldBe(IconColor.Primary);
        root.Href.ShouldBeNull();
        root.ParentId.ShouldBeNull();
    }

    [Theory]
    [InlineData("Authentication Check",  "/admin/authentication-check",  IconName.Lock,         IconColor.Secondary)]
    [InlineData("Site Config",           "/admin/site-configuration",    IconName.Briefcase,    IconColor.Secondary)]
    [InlineData("Scrape Directories",    "/admin/scrape-directories",    IconName.Globe,        IconColor.Secondary)]
    [InlineData("Models to Ignore",      "/admin/models-to-ignore",      IconName.GenderFemale, IconColor.Secondary)]
    [InlineData("Tags to Ignore",        "/admin/tags-to-ignore",        IconName.Tag,          IconColor.Secondary)]
    [InlineData("Add files to database", "/admin/add-files-to-database", IconName.Database,     IconColor.Secondary)]
    [InlineData("API Usage",             "/admin/api-usage",             IconName.AppIndicator, IconColor.Secondary)]
    public void ContainSpecificMenuItem(string id, string href, IconName iconName, IconColor iconColor)
    {
        var items = AdminMenuService.GetAdminMenuItems();

        var item = items.FirstOrDefault(i => i.Id == id);
        item.ShouldNotBeNull();
        item!.Href.ShouldBe(href);
        item.IconName.ShouldBe(iconName);
        item.IconColor.ShouldBe(iconColor);
        item.ParentId.ShouldBe("Admin");
    }

    [Fact]
    public void AllItemsShouldHaveUniqueIds()
    {
        var items = AdminMenuService.GetAdminMenuItems();

        var uniqueIds = items.Select(i => i.Id).Distinct().ToList();
        uniqueIds.Count.ShouldBe(items.Count);
    }

    [Fact]
    public void AllChildrenShouldReferenceAdminRoot()
    {
        var items = AdminMenuService.GetAdminMenuItems();

        var children = items.Where(i => i.Id != "Admin").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.ParentId == "Admin").ShouldBeTrue();
    }

    [Fact]
    public void AllChildrenShouldUseSecondaryIconColor()
    {
        var items = AdminMenuService.GetAdminMenuItems();

        var children = items.Where(i => i.Id != "Admin").ToList();
        children.ShouldNotBeEmpty();
        children.All(c => c.IconColor == IconColor.Secondary).ShouldBeTrue();
    }
}
