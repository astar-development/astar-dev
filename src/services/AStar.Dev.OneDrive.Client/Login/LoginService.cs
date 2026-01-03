using System.Diagnostics;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.ApplicationConfiguration;
using AStar.Dev.OneDrive.Client.User;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace AStar.Dev.OneDrive.Client.Login;

public sealed class LoginService(ApplicationSettings settings, UserPreferenceService userPreferenceService, ILogger<LoginService> logger) : ILoginService
{
    private GraphServiceClient? _client;
    private IPublicClientApplication _pca = null!;

    public async Task<Result<GraphServiceClient, Exception>> CreateGraphServiceClientAsync()
    {
        logger.LogInformation("Starting sign-in for ClientId={ClientId}, RememberMe={RememberMe}, CacheTag={CacheTag}",
            settings.ClientId, userPreferenceService.Load().UiSettings.RememberMe, settings.CacheTag);

        if (_client != null)
        {
            logger.LogDebug("Already signed in; returning existing Graph client");
            return new Result<GraphServiceClient, Exception>.Ok(_client);
        }

        return await Try.RunAsync(async () =>
        {
            _pca = PublicClientApplicationBuilder.Create(settings.ClientId)
                .WithTenantId(settings.TenantId)
                .WithRedirectUri("http://localhost")
                .Build();

            var cacheFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                $"AStarOneDriveTokenCache_v{settings.CacheTag}.bin");

            TokenCacheHelper.EnableSerialization(_pca.UserTokenCache, cacheFile);

            IEnumerable<IAccount> accounts = await _pca.GetAccountsAsync();
            IAccount? account = accounts.FirstOrDefault();

            AuthenticationResult result;
            if (account != null)
            {
                try
                {
                    result = await _pca.AcquireTokenSilent(settings.Scopes, account)
                        .ExecuteAsync();
                    logger.LogInformation("Silent token acquisition succeeded, expires {ExpiresOn}", result.ExpiresOn);
                }
                catch (MsalUiRequiredException)
                {
                    result = await _pca.AcquireTokenInteractive(settings.Scopes)
                        .WithPrompt(Prompt.SelectAccount)
                        .ExecuteAsync();
                    logger.LogInformation("Interactive token acquisition succeeded, expires {ExpiresOn}", result.ExpiresOn);
                }
            }
            else
            {
                result = await _pca.AcquireTokenInteractive(settings.Scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync();
                logger.LogInformation("Interactive token acquisition succeeded, expires {ExpiresOn}", result.ExpiresOn);
            }

            var provider = new MsalAuthenticationProvider(_pca, settings.Scopes);
            _client = new GraphServiceClient(provider);

            return _client;
        });
    }

    public Task<Result<bool, Exception>> SignOutAsync(bool hard = false)
        => Try.RunAsync(() =>
        {
            logger.LogInformation("Signing out user (RememberMe={RememberMe}, CacheTag={CacheTag})",
                userPreferenceService.Load().UiSettings.RememberMe, settings.CacheTag);

            var logoutUri =
                $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString("http://localhost")}";
            try
            {
                var psi = new ProcessStartInfo { FileName = logoutUri, UseShellExecute = true };
                _ = Process.Start(psi);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to open browser logout URL");
            }

            _client = null;

            if (!hard || !userPreferenceService.Load().UiSettings.RememberMe) return Task.FromResult(true);
            settings.CacheTag++;
            logger.LogInformation("Rotated cache tag to {CacheTag}", settings.CacheTag);

            return Task.FromResult(true);
        });
}
