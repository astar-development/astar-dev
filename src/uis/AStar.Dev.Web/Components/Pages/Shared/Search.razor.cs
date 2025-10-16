using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AStar.Dev.Web.Components.Pages.Shared;

public partial class Search : ComponentBase
{
    [SupplyParameterFromForm(FormName = "search")]
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
    private SearchModel SearchModel { get; set; } = new();

    [Parameter] public EventCallback<SearchModel> OnValidSubmit { get; set; }

    private string GetDaysText(int days) => days == 0 ? "Include all" : $"{days} days";

    private IEnumerable<SearchType> SearchTypeOptions => Enum.GetValues<SearchType>();

    private string GetSearchTypeText(SearchType searchType) => searchType switch
    {
        SearchType.Images => "Images",
        SearchType.All => "All Files",
        SearchType.Duplicates => "Duplicates",
        SearchType.DuplicateImages => "Duplicate Images",
        _ => searchType.ToString()
    };

    private async Task HandleFormSubmit(EditContext context) => await OnValidSubmit.InvokeAsync(SearchModel);
}
