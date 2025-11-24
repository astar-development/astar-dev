using System.Diagnostics;
using Azure.Identity;
using AStar.Dev.Functional.Extensions;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.OneDrive.Client.Services;

public class LoginService : ILoginService
{
    private readonly AppSettings _settings;
    private readonly ILogger<LoginService> _logger;
    private readonly Func<GraphServiceClient>? _graphClientFactory;
    private GraphServiceClient? _client;
    private InteractiveBrowserCredential? _credential;

    public LoginService(AppSettings settings, ILogger<LoginService> logger, Func<GraphServiceClient>? graphClientFactory = null)
    {
        _settings = settings;
        _logger = logger;
        _graphClientFactory = graphClientFactory;
    }

    public bool IsSignedIn => _client != null;

    public Task<Result<GraphServiceClient, Exception>> SignInAsync()
    {
        _logger.LogInformation("Starting interactive sign-in for ClientId={ClientId}", _settings.ClientId);

        if (_client != null)
        {
            _logger.LogDebug("Already signed in; returning existing Graph client");
            return Task.FromResult<Result<GraphServiceClient, Exception>>(new Result<GraphServiceClient, Exception>.Ok(_client));
        }

        return AStar.Dev.Functional.Extensions.Try.RunAsync(async () =>
        {
            if (_graphClientFactory != null)
            {
                _client = _graphClientFactory();
            }
            else
            {
                var options = new InteractiveBrowserCredentialOptions { ClientId = _settings.ClientId, TenantId = _settings.TenantId, RedirectUri = new Uri("http://localhost") };

                _credential = new InteractiveBrowserCredential(options);

                string[] scopes = { "User.Read", "Files.ReadWrite.All", "offline_access" };

                _client = new GraphServiceClient(_credential, scopes);
            }

            // Verify sign-in
            Microsoft.Graph.Models.User? me = await _client.Me.GetAsync();
            _logger.LogInformation("Sign-in completed for {DisplayName} ({Id})", me?.DisplayName, me?.Id);

            return _client;
        });
    }

    // Sign out and return Result<bool, Exception>
    public Task<Result<bool, Exception>> SignOutAsync()
    {
        return AStar.Dev.Functional.Extensions.Try.RunAsync(async () =>
        {
            _logger.LogInformation("Signing out user and clearing local credentials");

            // 1) Open browser logout to clear the browser session
            var logoutUri = $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString("http://localhost")}";
            try
            {
                // cross-platform process start
                var psi = new ProcessStartInfo { FileName = logoutUri, UseShellExecute = true };
                _ = Process.Start(psi);
                _logger.LogDebug("Opened browser logout URL");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to open browser logout URL (best-effort)");
            }

            // 2) Clear local references so the next operation forces a fresh login
            _credential = null;
            _client = null;

            _logger.LogDebug("Local credential references cleared");

            return true;
        });
    }
}
