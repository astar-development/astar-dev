using AStar.Dev.OneDrive.Client;
using AStar.Dev.OneDrive.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly OneDriveService _oneDriveService;
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty] private string _status = "Not signed in";

    public MainWindowViewModel(ILoginService loginService, OneDriveService oneDriveService, ILogger<MainWindowViewModel> logger)
    {
        _loginService = loginService;
        _oneDriveService = oneDriveService;
        _logger = logger;
        SignInCommand = new AsyncRelayCommand(SignInAsync);
    }

    public IAsyncRelayCommand SignInCommand { get; }

    private async Task SignInAsync()
    {
        try
        {
            _logger?.LogInformation("Sign-in started");
            GraphServiceClient client = await _loginService.SignInAsync();

            User? me = await client.Me.GetAsync();
            var driveType = client.Me.Drive.GetType().FullName;

            Status = $"Signed in as {me?.DisplayName} - {driveType}";
            _logger?.LogInformation("Sign-in succeeded for {Account}", me?.UserPrincipalName ?? me?.DisplayName ?? "unknown");
            List<DriveItem> x = await _oneDriveService.GetRootItemsAsync();
            
            foreach (DriveItem item in x) Console.WriteLine(item.Name);
        }
        catch(Exception ex)
        {
            Status = $"Login failed: {ex.Message}";
            _logger?.LogError(ex, "Sign-in failed");
        }
    }
}
