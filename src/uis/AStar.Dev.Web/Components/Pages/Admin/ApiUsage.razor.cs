using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace AStar.Dev.Web.Components.Pages.Admin;

public partial class ApiUsage : ComponentBase
{
    /// <inheritdoc />
    [Authorize]
    protected override Task OnInitializedAsync() => base.OnInitializedAsync();
}
