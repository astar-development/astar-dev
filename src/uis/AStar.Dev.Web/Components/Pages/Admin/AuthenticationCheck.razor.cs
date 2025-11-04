using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AStar.Dev.Web.Components.Pages.Admin;

public partial class AuthenticationCheck : ComponentBase
{
    private string             _authMessage = "Not Authorized - default message";
    private IEnumerable<Claim> _claims      = [];

    [Inject]
    public required AuthenticationStateProvider AuthenticationStateProvider { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync() => await GetClaimsPrincipalData();

    /// <summary>
    ///     Retrieves the user claims for the signed-in user.
    /// </summary>
    /// <returns></returns>
    private async Task GetClaimsPrincipalData()
    {
        AuthenticationState authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

        ClaimsPrincipal user = authState.User;

        if(user.Identity is
           {
               IsAuthenticated: true
           })
        {
            _authMessage = $"{user.Identity.Name} is authenticated.";

            _claims = user.Claims; //.Where(x => printClaims.Contains(x.Type)); // The Where will, as you can guess, limit the results listed
        }
        else
            _authMessage = "The user is NOT authenticated.";
    }
}
