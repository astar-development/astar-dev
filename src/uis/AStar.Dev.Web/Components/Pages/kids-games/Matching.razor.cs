using BlazorBootstrap;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.kids_games;

[UsedImplicitly]
public partial class Matching : ComponentBase
{
    [Inject]
    private ILogger<Matching> Logger { get; set; } = null!;

    private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized()
    {
        Logger.LogInformation("Page: {PageName} viewed", nameof(Matching));

        NavItems1 =
        [
            new() { Text = "Home", Href              = "/" },
            new() { Text = "Kids Games", Href        = "kids-games" },
            new() { Text = "Matching", IsCurrentPage = true }
        ];
    }
}
