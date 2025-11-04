using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.KidsGames.matching;

[UsedImplicitly]
public partial class MatchHouses : ComponentBase
{
    [Inject]
    private ILogger<MatchHouses> Logger { get; set; } = null!;

    // private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized() => Logger.LogInformation("Page: {PageName} viewed", nameof(MatchHouses));

    // NavItems1 =
    // [
    //     new() { Text = "Home", Href                  = "/" },
    //     new() { Text = "Kids Games", Href            = "kids-games" },
    //     new() { Text = "Matching", Href              = "kids-games/matching" },
    //     new() { Text = "Match Houses", IsCurrentPage = true }
    // ];
}
