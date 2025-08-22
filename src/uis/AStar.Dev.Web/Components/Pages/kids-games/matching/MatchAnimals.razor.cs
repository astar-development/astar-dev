using BlazorBootstrap;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.kids_games.matching;

[UsedImplicitly]
public partial class MatchAnimals : ComponentBase
{
    [Inject]
    private ILogger<MatchAnimals> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(MatchAnimals));

        NavItems1 =
        [
            new() { Text = "Home", Href                   = "/" },
            new() { Text = "Kids Games", Href             = "kids-games" },
            new() { Text = "Matching", Href               = "kids-games/matching" },
            new() { Text = "Match Animals", IsCurrentPage = true }
        ];
    }
}
