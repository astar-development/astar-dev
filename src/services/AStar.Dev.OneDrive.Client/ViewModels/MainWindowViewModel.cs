using AStar.Dev.OneDrive.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Collections.ObjectModel;
using AStar.Dev.Functional.Extensions;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly OneDriveService _oneDriveService;
    private readonly ILogger<MainWindowViewModel> _logger;

    [ObservableProperty] private string _status = "Not signed in";
    [ObservableProperty] private string _errorMessage = string.Empty;

    public MainWindowViewModel(ILoginService loginService, OneDriveService oneDriveService, ILogger<MainWindowViewModel> logger)
    {
        _loginService = loginService;
        _oneDriveService = oneDriveService;
        _logger = logger;
        SignInCommand = new AsyncRelayCommand(SignInAsync);
        LoadRootCommand = new AsyncRelayCommand(LoadRootItemsAsync);
    }

    public IAsyncRelayCommand SignInCommand { get; }
    public IAsyncRelayCommand LoadRootCommand { get; }

    [ObservableProperty] private ObservableCollection<DriveItem> _rootItems = new();

    private async Task SignInAsync()
    {
        try
        {
            _logger?.LogInformation("Sign-in started");
            ErrorMessage = string.Empty;
            GraphServiceClient client = await _loginService.SignInAsync();
            User? me = await client.Me.GetAsync();
            var driveType = client.Me.Drive.GetType().FullName;

            Status = $"Signed in as {me?.DisplayName} - {driveType}";
            _logger?.LogInformation("Sign-in succeeded for {Account}", me?.UserPrincipalName ?? me?.DisplayName ?? "unknown");

            var rootResult = await _oneDriveService.GetRootItemsAsync();
            if (rootResult is Result<List<DriveItem>, Exception>.Ok ok)
            {
                foreach (DriveItem item in ok.Value) Console.WriteLine(item.Name);
                ErrorMessage = string.Empty;
            }
            else if (rootResult is Result<List<DriveItem>, Exception>.Error err)
            {
                ErrorMessage = err.Reason.Message;
                _logger?.LogError(err.Reason, "Failed to list root items after sign-in");
            }
        }
        catch(Exception ex)
        {
            Status = $"Login failed: {ex.Message}";
            ErrorMessage = ex.Message;
            _logger?.LogError(ex, "Sign-in failed");
        }
    }

    private async Task LoadRootItemsAsync()
    {
        try
        {
            _logger?.LogInformation("Loading OneDrive root items");
            ErrorMessage = string.Empty;
            var result = await _oneDriveService.GetRootItemsAsync();
            if (result is Result<List<DriveItem>, Exception>.Ok ok2)
            {
                RootItems = new ObservableCollection<DriveItem>(ok2.Value);
                Status = $"Loaded {RootItems.Count} item(s)";
            }
            else if (result is Result<List<DriveItem>, Exception>.Error err2)
            {
                Status = $"Load failed: {err2.Reason.Message}";
                ErrorMessage = err2.Reason.Message;
                _logger?.LogError(err2.Reason, "Failed to load root items");
            }
        }
        catch (Exception ex)
        {
            Status = $"Load failed: {ex.Message}";
            ErrorMessage = ex is OneDriveService.OneDriveServiceException odse ? $"OneDrive error ({odse.StatusCode}): {odse.Message}" : ex.Message;
            _logger?.LogError(ex, "Failed to load root items");
        }
    }
}
