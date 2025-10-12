using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Images;

public partial class DuplicateImages : ComponentBase
{
    private readonly JustifyContent _justifyContent = JustifyContent.FlexStart;
    private readonly int            _spacing        = 3;

    [SupplyParameterFromForm]
    private SearchModel SearchModel { get; } = new();

    private void HandleValidSubmit()
    {
    }

    private void OnBreakpointEnterHandler(GridItemSize size) => Console.WriteLine($"Page Size: {size}");
}
