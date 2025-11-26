using System.Diagnostics;
using AStar.Dev.Functional.Extensions;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Authentication;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class LoginService(AppSettings settings, UserSettings userSettings, ILogger<LoginService> logger) : ILoginService
{
    private GraphServiceClient? _client;
    private InteractiveBrowserCredential? _credential;

    public bool IsSignedIn => _client != null;

    public Task<Result<GraphServiceClient, Exception>> SignInAsync()
    {
        logger.LogInformation("Starting sign-in for ClientId={ClientId}, RememberMe={RememberMe}, CacheTag={CacheTag}",
            settings.ClientId, userSettings.RememberMe, userSettings.CacheTag);

        if(_client != null)
        {
            logger.LogDebug("Already signed in; returning existing Graph client");
            return Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Ok(_client));
        }

        return Try.RunAsync(async () =>
        {
            InteractiveBrowserCredentialOptions options = BuildCredentialOptions();

            var allowedHosts = new[] { "graph.microsoft.com" };
            var graphScopes = new[] { "User.Read", "Files.ReadWrite.All", "offline_access" };

            _credential = new InteractiveBrowserCredential(options);

            var authProvider = new AzureIdentityAuthenticationProvider(
                _credential,
                allowedHosts,
                null,
                true,
                scopes: graphScopes);

            _client = GraphClientFactory.CreateGraphClient(authProvider);
            return _client;
        });
    }

    public Task<Result<bool, Exception>> SignOutAsync(bool hard = false)
        => Try.RunAsync<bool>(() =>
        {
            logger.LogInformation("Signing out user (RememberMe={RememberMe}, CacheTag={CacheTag})",
                userSettings.RememberMe, userSettings.CacheTag);

            // 1) Browser logout
            var logoutUri =
                $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString("http://localhost")}";
            try
            {
                var psi = new ProcessStartInfo { FileName = logoutUri, UseShellExecute = true };
                _ = Process.Start(psi);
            }
            catch(Exception ex)
            {
                logger.LogWarning(ex, "Failed to open browser logout URL");
            }

            // 2) Clear local references
            _credential = null;
            _client = null;

            // 3) Hard sign-out: rotate cache name
            if(hard && userSettings.RememberMe)
            {
                userSettings.CacheTag++;
                logger.LogInformation("Rotated cache tag to {CacheTag}", userSettings.CacheTag);
            }

            return Task.FromResult(true);
        });

    private InteractiveBrowserCredentialOptions BuildCredentialOptions()
    {
        var options = new InteractiveBrowserCredentialOptions
        {
            ClientId = settings.ClientId,
            TenantId = settings.TenantId,
            RedirectUri = new Uri("http://localhost"),
        };

        if(userSettings.RememberMe)
        {
            options.TokenCachePersistenceOptions = new TokenCachePersistenceOptions
            {
                Name = $"MyAppTokenCache_v{userSettings.CacheTag}",
                UnsafeAllowUnencryptedStorage = false
            };
        }

        return options;
    }
}
