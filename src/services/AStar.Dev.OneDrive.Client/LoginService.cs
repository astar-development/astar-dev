using System.Diagnostics;
using Azure.Identity;
using Microsoft.Graph;

namespace AStar.Dev.OneDrive.Client;

public class LoginService(AppSettings settings) : ILoginService
{
    private GraphServiceClient? _client;
    private InteractiveBrowserCredential? _credential;

    public bool IsSignedIn => _client != null;

    public async Task<GraphServiceClient> SignInAsync()
    {
        if(_client != null) return _client;

        var options = new InteractiveBrowserCredentialOptions { ClientId = settings.ClientId, TenantId = settings.TenantId, RedirectUri = new Uri("http://localhost") };

        _credential = new InteractiveBrowserCredential(options);

        string[] scopes = { "User.Read", "Files.ReadWrite.All", "offline_access" };

        _client = new GraphServiceClient(_credential, scopes);

        // Verify sign-in
        await _client.Me.GetAsync();

        return _client;
    }

    public Task SignOutAsync()
    {
        // 1) Open browser logout to clear the browser session
        var logoutUri = $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={Uri.EscapeDataString("http://localhost")}";
        try
        {
            // cross-platform process start
            var psi = new ProcessStartInfo { FileName = logoutUri, UseShellExecute = true };
            Process.Start(psi);
        }
        catch
        {
            // best-effort; swallow but log in real app
        }

        // 2) Clear local references so the next operation forces a fresh login
        _credential = null;
        _client = null;

        return Task.CompletedTask;
    }
}
