using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.KidsGames.halving;

[UsedImplicitly]
public partial class Halving4 : ComponentBase
{
    [Inject]
    private ILogger<Halving4> Logger { get; set; } = null!;

    // private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized() => Logger.LogInformation("Page: {PageName} viewed", nameof(Halving4));

    // NavItems1 =
    // [
    //     new() { Text = "Home", Href                  = "/" },
    //     new() { Text = "Kids Games", Href            = "kids-games" },
    //     new() { Text = "Halving", Href               = "kids-games/halving" },
    //     new() { Text = "Halving Four", IsCurrentPage = true }
    // ];
}
