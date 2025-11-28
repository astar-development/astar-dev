using System.Diagnostics;
using AStar.Dev.Functional.Extensions;
using Azure.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class LoginService(AppSettings settings, UserSettings userSettings, ILogger<LoginService> logger) : ILoginService
{
    private GraphServiceClient? _client;
    private IPublicClientApplication _pca = null!;
    private readonly string[] _scopes = new[] { "User.Read", "Files.ReadWrite.All", "offline_access" };

public async Task<Result<GraphServiceClient, Exception>> CreateGraphServiceClientAsync()
{
    logger.LogInformation("Starting sign-in for ClientId={ClientId}, RememberMe={RememberMe}, CacheTag={CacheTag}",
        settings.ClientId, userSettings.RememberMe, userSettings.CacheTag);

    if (_client != null)
    {
        logger.LogDebug("Already signed in; returning existing Graph client");
        return new Result<GraphServiceClient, Exception>.Ok(_client);
    }

    return Try.Run(() =>
    {
        _pca = PublicClientApplicationBuilder.Create(settings.ClientId)
            .WithTenantId(settings.TenantId)
            .WithRedirectUri("http://localhost")
            .Build();

        var cacheFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            $"AStarOneDriveTokenCache_v{userSettings.CacheTag}.bin");

        TokenCacheHelper.EnableSerialization(_pca.UserTokenCache, cacheFile);

        IEnumerable<IAccount> accounts = _pca.GetAccountsAsync().GetAwaiter().GetResult();
        IAccount? account = accounts.FirstOrDefault();

        AuthenticationResult result;
        if (account != null)
        {
            try
            {
                result = _pca.AcquireTokenSilent(_scopes, account)
                             .ExecuteAsync().GetAwaiter().GetResult();
                logger.LogInformation("Silent token acquisition succeeded, expires {ExpiresOn}", result.ExpiresOn);
            }
            catch (MsalUiRequiredException)
            {
                result = _pca.AcquireTokenInteractive(_scopes)
                             .WithPrompt(Prompt.SelectAccount)
                             .ExecuteAsync().GetAwaiter().GetResult();
                logger.LogInformation("Interactive token acquisition succeeded, expires {ExpiresOn}", result.ExpiresOn);
            }
        }
        else
        {
            result = _pca.AcquireTokenInteractive(_scopes)
                         .WithPrompt(Prompt.SelectAccount)
                         .ExecuteAsync().GetAwaiter().GetResult();
            logger.LogInformation("Interactive token acquisition succeeded, expires {ExpiresOn}", result.ExpiresOn);
        }

        var provider = new MsalAuthenticationProvider(_pca, _scopes);
        _client = new GraphServiceClient(provider);

        return _client;
    });
}

    public Task<Result<bool, Exception>> SignOutAsync(bool hard = false)
        => Try.RunAsync<bool>(() =>
        {
            logger.LogInformation("Signing out user (RememberMe={RememberMe}, CacheTag={CacheTag})",
                userSettings.RememberMe, userSettings.CacheTag);

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

            _client = null;

            if(hard && userSettings.RememberMe)
            {
                userSettings.CacheTag++;
                logger.LogInformation("Rotated cache tag to {CacheTag}", userSettings.CacheTag);
            }

            return Task.FromResult(true);
        });
}
