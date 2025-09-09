using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AStar.Dev.Web.Components.Layout;

public partial class LoginDisplay : ComponentBase
{
    private string _name = "Unknown";

    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    protected override async Task OnInitializedAsync() =>
        await GetClaimsPrincipalData();

    /// <summary>
    ///     Retrieves the user claims for the signed-in user.
    /// </summary>
    /// <returns></returns>
    private async Task GetClaimsPrincipalData()
    {
        var user = (await AuthenticationStateProvider.GetAuthenticationStateAsync()).User;

        if(user.Identity is
           {
               IsAuthenticated: true
           })
        {
            _name = user.Claims.Single(claim => claim.Type == "name").Value.Split(' ')[0];
        }
    }
}
