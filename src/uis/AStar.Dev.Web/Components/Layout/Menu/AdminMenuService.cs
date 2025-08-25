using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

public static class AdminMenuService
{
    private const string AdminRoot = "Admin";

    public static List<NavItem> GetAdminMenuItems() =>
    [
        new() { Id = AdminRoot, IconName = IconName.LayoutSidebarInset, Text = AdminRoot, IconColor = IconColor.Primary },
        new()
        {
            Id        = "Authentication Check",
            Href      = "/admin/authentication-check",
            IconName  = IconName.Lock,
            IconColor = IconColor.Secondary,
            Text      = "Authentication Check",
            ParentId  = AdminRoot
        },
        new()
        {
            Id        = "Site Config",
            Href      = "/admin/site-configuration",
            IconName  = IconName.Briefcase,
            IconColor = IconColor.Secondary,
            Text      = "Site Config",
            ParentId  = AdminRoot
        },
        new()
        {
            Id        = "Scrape Directories",
            Href      = "/admin/scrape-directories",
            IconColor = IconColor.Secondary,
            IconName  = IconName.Globe,
            Text      = "Scrape Directories",
            ParentId  = AdminRoot
        },
        new()
        {
            Id        = "Models to Ignore",
            Href      = "/admin/models-to-ignore",
            IconColor = IconColor.Secondary,
            IconName  = IconName.GenderFemale,
            Text      = "Models to Ignore",
            ParentId  = AdminRoot
        },
        new()
        {
            Id        = "Tags to Ignore",
            Href      = "/admin/tags-to-ignore",
            IconColor = IconColor.Secondary,
            IconName  = IconName.Tag,
            Text      = "Tags to Ignore",
            ParentId  = AdminRoot
        },
        new()
        {
            Id        = "Add files to database",
            Href      = "/admin/add-files-to-database",
            IconColor = IconColor.Secondary,
            IconName  = IconName.Database,
            Text      = "Add files to database",
            ParentId  = AdminRoot
        },
        new()
        {
            Id        = "API Usage",
            Href      = "/admin/api-usage",
            IconColor = IconColor.Secondary,
            IconName  = IconName.AppIndicator,
            Text      = "API Usage",
            ParentId  = AdminRoot
        }
    ];
}
