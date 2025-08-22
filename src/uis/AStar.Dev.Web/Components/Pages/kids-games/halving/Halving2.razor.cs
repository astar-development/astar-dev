using BlazorBootstrap;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.kids_games.halving;

public partial class Halving2
{
    [Inject]
    private ILogger<Halving2> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(Halving2));

        NavItems1 =
        [
            new() { Text = "Home", Href                 = "/" },
            new() { Text = "Kids Games", Href           = "kids-games" },
            new() { Text = "Halving", Href              = "kids-games/halving" },
            new() { Text = "Halving Two", IsCurrentPage = true }
        ];
    }
}
