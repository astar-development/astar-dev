using AStar.Dev.Files.Api.Client.SDK.FilesApi;
using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SearchType = AStar.Dev.Web.Models.SearchType;
using SortOrder = AStar.Dev.Web.Models.SortOrder;

namespace AStar.Dev.Web.Components.Pages.Shared;

public partial class Search : ComponentBase
{
    [Inject] private FilesApiClient FilesApiClient { get; set; } = null!;

    private List<FileClassification> FileClassifications { get; set; } = [];

    [SupplyParameterFromForm(FormName = "search")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    private SearchModel SearchModel { get; set; } = new();

    [Parameter] public EventCallback<SearchModel> OnValidSubmit { get; set; }

    private static string GetDaysText(int days) => days == 0 ? "Include all" : $"{days} days";

    private static IEnumerable<SearchType> SearchTypeOptions => Enum.GetValues<SearchType>();
    private static IEnumerable<SortOrder> SortOrderOptions => Enum.GetValues<SortOrder>();

    private static string GetSearchTypeText(SearchType searchType) => searchType switch
    {
        SearchType.Images => "Images",
        SearchType.All => "All Files",
        SearchType.Duplicates => "Duplicates",
        SearchType.DuplicateImages => "Duplicate Images",
        _ => searchType.ToString()
    };

    private static string GetSearchOrderText(SortOrder sortOrder) => sortOrder switch
    {
        SortOrder.NameAscending => "Name Ascending",
        SortOrder.NameDescending => "Name Descending",
        SortOrder.SizeDescending => "Size Descending",
        SortOrder.SizeAscending => "Size Ascending",
        _ => sortOrder.ToString()
    };

    protected override async Task OnInitializedAsync()
    {
        // Add a placeholder "no selection" option
        FileClassifications = new() { new() { Id = Guid.Empty, Name = "-- Select --", IncludeInSearch = false, Celebrity = false } };

        var apiClassifications = await FilesApiClient.GetFileClassificationsAsync();

        FileClassifications.AddRange(apiClassifications);
    }

    private async Task HandleFormSubmit(EditContext context) => await OnValidSubmit.InvokeAsync(SearchModel);
}
