using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Images;

[UsedImplicitly]
public partial class DuplicateImages : ComponentBase
{
    [SupplyParameterFromForm]
    private SearchModel SearchModel { get; } = new();

    private void HandleValidSubmit(SearchModel model)
    {
        // Your logic here with the model directly
    }
}
