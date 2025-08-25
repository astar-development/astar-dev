using BlazorBootstrap;

namespace AStar.Dev.Web.Components.Layout.Menu;

/// <summary>
///     Represents a service for retrieving menu items to be used in the application's navigation.
/// </summary>
public interface IMenuItemsService
{
    /// <summary>
    ///     Retrieves a collection of navigation items to be used in the application's menu.
    /// </summary>
    /// <returns>
    ///     A collection of <see cref="NavItem" /> representing the menu structure.
    /// </returns>
    /// <example>
    ///     The following example demonstrates how to use the <c>GetNavItems</c> method:
    ///     <code>
    /// IMenuItemsService menuItemsService = new MenuItemsService();
    /// IEnumerable&lt;NavItem&gt; navItems = menuItemsService.GetNavItems();
    /// foreach (var navItem in navItems)
    /// {
    /// Console.WriteLine($"Id: {navItem.Id}, Text: {navItem.Text}, Href: {navItem.Href}");
    /// }
    /// </code>
    /// </example>
    IEnumerable<NavItem> GetNavItems();
}
