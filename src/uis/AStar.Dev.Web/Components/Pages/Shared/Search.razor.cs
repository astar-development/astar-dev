using AStar.Dev.Web.Models;
using AStar.Dev.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using SearchType = AStar.Dev.Web.Models.SearchType;
using SortOrder = AStar.Dev.Web.Models.SortOrder;

namespace AStar.Dev.Web.Components.Pages.Shared;

public partial class Search : ComponentBase
{
    private readonly string _gridClass = "background-color: var(--neutral-layer-3); padding: 4px; ";
    private List<FileClassification> FileClassifications { get; set; } = [];

    [Inject] private IFileClassificationsService FileClassificationsService { get; set; } = null!;

    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;

    [SupplyParameterFromForm(FormName = "search")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    public required SearchModel SearchModel { get; set; }

    [Parameter] public EventCallback<SearchModel> OnValidSubmit { get; set; }

    private static IEnumerable<SearchType> SearchTypeOptions => Enum.GetValues<SearchType>();

    private static IEnumerable<SortOrder> SortOrderOptions => Enum.GetValues<SortOrder>();

    private static string GetDaysText(int days) => days == 0 ? "Include all" : $"{days} days";

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
        FileClassifications =
        [
            // new FileClassification(new Dev.Files.Classifications.Api.FileClassification(Guid.Empty, 1, "-- Select (Optional) --", null,null, false, false)
            // )
        ];

        IEnumerable<FileClassification> classifications = await FileClassificationsService.GetFileClassificationsAsync();
        FileClassifications.AddRange(classifications);
    }

    private async Task HandleFormSubmit(EditContext context) => await OnValidSubmit.InvokeAsync(SearchModel);
}
