using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AStar.Dev.Web.Components.Layout;

public partial class NavMenu : ComponentBase
{
    private bool _expanded = true;
    private string? _firstName;
    private bool _isAuthenticated;

    [CascadingParameter] private Task<AuthenticationState> AuthenticationStateTask { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        var authState = await AuthenticationStateTask;
        var user = authState.User;
        _isAuthenticated = user.Identity?.IsAuthenticated ?? false;
        _firstName = _isAuthenticated ? ExtractFirstName(user) : null;
    }

    private static string ExtractFirstName(ClaimsPrincipal user)
    {
        string[] claimTypes = [ClaimTypes.GivenName, "given_name", "name", ClaimTypes.Name, "preferred_username", ClaimTypes.Email];

        return GetNameClaim(user, claimTypes) switch
        {
            var value when value.Contains('@') => value.AsSpan()[..value.IndexOf('@')].ToString(),
            var value when value.Contains(' ') => value.AsSpan()[..value.IndexOf(' ')].ToString(),
            var value => value
        };
    }

    private static string GetNameClaim(ClaimsPrincipal user, string[] claimTypes)
        => claimTypes
               .Select(user.FindFirst)
               .FirstOrDefault(c => !string.IsNullOrWhiteSpace(c?.Value))?.Value
           ?? IdentityNameOrDefault(user);

    private static string IdentityNameOrDefault(ClaimsPrincipal user) => user.Identity?.Name ?? "User";
}
