using AStar.Dev.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace AStar.Dev.Web.Components.Pages;

public partial class Search : ComponentBase
{
    [SupplyParameterFromForm] private SearchModel SearchModel { get; } = new();

    [Parameter] public EventCallback<SearchModel> OnValidSubmit { get; set; }

    private async Task HandleFormSubmit(EditContext context) => await OnValidSubmit.InvokeAsync(SearchModel);
}
