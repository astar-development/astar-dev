using AStar.Dev.Functional.Extensions;
using AStar.Dev.Web.Services;
using FluentUI.Demo.Shared.SampleData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Admin;

[Authorize(Policy = "AdminOnly")]
public partial class FileClassifications(IFileClassificationsService fileClassificationsService, ILogger<FileClassifications> logger) : ComponentBase
{
    private FluentDataGrid<FoodRecall> _dataGrid = null!;
    private IQueryable<FoodRecall> _foodRecallItems = null!;
    private bool _loading = true;
    private string? _stateFilter = "NY";

    protected async Task RefreshItemsAsync(GridItemsProviderRequest<FoodRecall> req)
    {
        logger.LogInformation("Refreshing food recall items with request: {@Request}", req);
        _loading = true;
        await InvokeAsync(StateHasChanged);
        _ = await fileClassificationsService.GetFileClassificationsAsync()
            .TapAsync(fileClassifications => logger.LogInformation("Retrieved {Count} file classifications", fileClassifications.Count));

        var filters = new Dictionary<string, object?>
        {
            { "skip", req.StartIndex },
            { "limit", req.Count }
        };

        if(!string.IsNullOrWhiteSpace(_stateFilter)) filters.Add("search", $"state:{_stateFilter}");

        SortedProperty s = req.GetSortByProperties().FirstOrDefault();
        if(req.SortByColumn != null && !string.IsNullOrEmpty(s.PropertyName)) filters.Add("sort", s.PropertyName + (s.Direction == SortDirection.Ascending ? ":asc" : ":desc"));

#pragma warning disable S1075 // URIs should not be hardcoded
        var url = NavManager.GetUriWithQueryParameters("https://api.fda.gov/food/enforcement.json", filters);
#pragma warning restore S1075 // URIs should not be hardcoded

        FoodRecallQueryResult? response = await Http.GetFromJsonAsync<FoodRecallQueryResult>(url);

        _foodRecallItems = response!.Results.AsQueryable();
        await _pagination.SetTotalItemCountAsync(response!.Meta.Results.Total);

        _loading = false;
        await InvokeAsync(StateHasChanged);
    }

    public void ClearFilters() => _stateFilter = null;

    public async Task DataGridRefreshDataAsync() => await _dataGrid.RefreshDataAsync(true);

    private readonly PaginationState _pagination = new() { ItemsPerPage = 20 };
}
