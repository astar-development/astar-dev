using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.kids_games;

public partial class Halving
{
    [Inject]
    private ILogger<Halving> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(Halving));

        NavItems1 =
        [
            new() { Text = "Home", Href             = "/" },
            new() { Text = "Kids Games", Href       = "kids-games" },
            new() { Text = "Halving", IsCurrentPage = true }
        ];
    }
}
