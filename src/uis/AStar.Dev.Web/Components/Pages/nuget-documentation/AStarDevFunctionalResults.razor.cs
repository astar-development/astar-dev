using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.nuget_documentation;

public partial class AStarDevFunctionalResults
{
    [Inject]
    private ILogger<AStarDevFunctionalResults> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(AStarDevFunctionalResults));

        NavItems1 =
        [
            new() { Text = "Home", Href                        = "/" },
            new() { Text = "Nuget Documentation", Href         = "nuget-documentation" },
            new() { Text = "Functional Results", IsCurrentPage = true }
        ];
    }
}
