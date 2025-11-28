using Microsoft.Identity.Client;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class MsalAuthenticationProvider : IAuthenticationProvider
{
    private readonly IPublicClientApplication _pca;
    private readonly string[] _scopes;

    public MsalAuthenticationProvider(IPublicClientApplication pca, string[] scopes)
    {
        _pca = pca;
        _scopes = scopes;
    }

    public async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync().ConfigureAwait(false);
        IAccount? account = accounts.FirstOrDefault();

        AuthenticationResult result;
        try
        {
            result = await _pca.AcquireTokenSilent(_scopes, account)
                .ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (MsalUiRequiredException)
        {
            result = await _pca.AcquireTokenInteractive(_scopes)
                .WithAccount(account)
                .WithPrompt(Prompt.SelectAccount)
                .ExecuteAsync(cancellationToken).ConfigureAwait(false);
        }

        // Set the Authorization header on the Kiota RequestInformation
        request.Headers.Add("Authorization", $"Bearer {result.AccessToken}");
    }
}
