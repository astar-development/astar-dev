using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages;

public partial class Home
{
    [Inject]
    private ILogger<Home> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(Home));

        NavItems1 =
        [
            new() { Text = "Home", Href                        = "/" },
            new() { Text = "Nuget Documentation", Href         = "nuget-documentation" },
            new() { Text = "Functional Results", IsCurrentPage = true }
        ];
    }
}
