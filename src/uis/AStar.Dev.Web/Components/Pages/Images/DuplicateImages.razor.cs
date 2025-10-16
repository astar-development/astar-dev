using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Images;

[UsedImplicitly]
public partial class DuplicateImages : ComponentBase
{
    private void HandleValidSubmit(SearchModel model)
    {
        var excludeDays = int.TryParse(model.ExcludeViewedWithinDays, out var days) ? days : 0;
        var searchType = Enum.TryParse<SearchType>(model.SearchType, out var type) ? type : SearchType.Duplicates;

        Console.WriteLine($"Form submitted! Directory: {model.StartingDirectory}, Type: {searchType}, Exclude: {excludeDays} days");
    }
}
