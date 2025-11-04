using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Images;

[UsedImplicitly]
public partial class DuplicateImages(ILogger<DuplicateImages> logger) : ComponentBase
{
    private void HandleValidSubmit(SearchModel model)
    {
        var excludeDays = int.TryParse(model.ExcludeViewedWithinDays, out var days) ? days : 0;
        SearchType searchType = Enum.TryParse<SearchType>(model.SearchType, out SearchType type) ? type : SearchType.All;

        logger.LogDebug(
            "Form submitted! Directory: {StartingDirectory}, Type: {SearchType}, Exclude: {ExcludeDays} days",
            model.StartingDirectory, searchType, excludeDays);
        Console.WriteLine($"Form submitted! Directory: {model.StartingDirectory}, Type: {searchType}, Exclude: {excludeDays} days");
    }
}
