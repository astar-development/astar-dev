using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.nuget_documentation;

[UsedImplicitly]
public class NuGetDocumentation : ComponentBase
{
    [Inject]
    private ILogger<NuGetDocumentation> Logger { get; set; } = null!;

    //private List<BreadcrumbItem> NavItems1 { get; set; } = null!;

    protected override void OnInitialized() => Logger.LogInformation("Page: {PageName} viewed", nameof(NuGetDocumentation));

    //     NavItems1 =
    //     [
    //         new() { Text = "Home", Href                = "/" },
    //         new() { Text = "Nuget Documentation", Href = "nuget-documentation", IsCurrentPage = true }
    //     ];
}
