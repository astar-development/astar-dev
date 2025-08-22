using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.kids_games;

public partial class Games
{
    [Inject]
    private ILogger<Games> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(Games));

        NavItems1 =
        [
            new() { Text = "Home", Href       = "/" },
            new() { Text = "Kids Games", Href = "kids-games", IsCurrentPage = true }
        ];
    }
}
