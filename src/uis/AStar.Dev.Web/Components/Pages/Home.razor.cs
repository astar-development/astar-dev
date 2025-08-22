using BlazorBootstrap;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages;

[UsedImplicitly]
public partial class Home : ComponentBase
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
