using BlazorBootstrap;

namespace AStar.Dev.Web.Services;

public class MenuItemsService
{
    public IEnumerable<NavItem> GetNavItems() =>
        new List<NavItem>
        {
            new() { Id = "2", IconName = IconName.LayoutSidebarInset, Text = "Content", IconColor = IconColor.Primary },
            new()
            {
                Id       = "3",
                Href     = "/icons",
                IconName = IconName.PersonSquare,
                Text     = "Icons",
                ParentId = "2"
            },
            new() { Id = "4", IconName = IconName.ExclamationTriangleFill, Text = "Components", IconColor = IconColor.Success },
            new()
            {
                Id       = "5",
                Href     = "/alerts",
                IconName = IconName.CheckCircleFill,
                Text     = "Alerts",
                ParentId = "4"
            },
            new()
            {
                Id       = "6",
                Href     = "/breadcrumb",
                IconName = IconName.SegmentedNav,
                Text     = "Breadcrumb",
                ParentId = "4"
            },
            new()
            {
                Id       = "7",
                Href     = "/sidebar",
                IconName = IconName.LayoutSidebarInset,
                Text     = "Sidebar",
                ParentId = "4"
            },
            new() { Id = "8", IconName = IconName.WindowPlus, Text = "Nuget Documentation", IconColor = IconColor.Danger },
            new()
            {
                Id       = "9",
                Href     = "/nuget-documentation",
                IconName = IconName.InputCursorText,
                Text     = "Root Docs",
                ParentId = "8"
            },
            new()
            {
                Id       = "10",
                Href     = "/currency-input",
                IconName = IconName.CurrencyDollar,
                Text     = "Currency Input",
                ParentId = "8"
            },
            new()
            {
                Id       = "11",
                Href     = "/number-input",
                IconName = IconName.InputCursor,
                Text     = "Number Input",
                ParentId = "8"
            },
            new()
            {
                Id       = "12",
                Href     = "/switch",
                IconName = IconName.ToggleOn,
                Text     = "Switch",
                ParentId = "8"
            },
            new() { Id = "13", Href = "/testing-dashboard", IconName = IconName.BugFill, Text = "Testing Dashboard" },
            new() { Id = "14", Href = "/kids-games", IconName        = IconName.Dice6, Text   = "Kids Games" }
        };
}
