using System.Collections.ObjectModel;
using System.Windows.Input;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.Services;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private CancellationTokenSource? _cts = new();
    private readonly ILoginService _loginService;
    private readonly OneDriveService _oneDriveService;
    private readonly ILogger<MainWindowViewModel> _logger;
    [ObservableProperty] private string _errorMessage = string.Empty;
    public ObservableCollection<string> ProgressMessages { get; } = new();
    public bool FollowLog
{
    get;
    set => SetProperty(ref field, value);
} = true;
    public bool DownloadFilesAfterSync
{
    get;
    set => SetProperty(ref field, value);
}

    public MainWindowViewModel(ILoginService loginService, OneDriveService oneDriveService, ILogger<MainWindowViewModel> logger)
    {
        _loginService = loginService;
        _oneDriveService = oneDriveService;
        _logger = logger;
        SignInCommand = new AsyncRelayCommand(SignInAsync);
        SignOutCommand = new AsyncRelayCommand(SignOutAsync);
        LoadRootCommand = new AsyncRelayCommand(LoadRootItemsAsync);
    }
    public void ReportProgress(string message, double? progress = null, string? status = null) => Dispatcher.UIThread.Post(() =>
                                                                                                       {
                                                                                                           ProgressMessages.Add($"{DateTime.Now:T} - {message}");
                                                                                                           if(progress.HasValue)
                                                                                                               ProgressValue = progress.Value;
                                                                                                           if(!string.IsNullOrEmpty(status))
                                                                                                               Status = status;
                                                                                                       });
    public IAsyncRelayCommand SignInCommand { get; }
    public IAsyncRelayCommand SignOutCommand { get; }
    public IAsyncRelayCommand LoadRootCommand { get; }
    public IAsyncRelayCommand CancelSyncCommand => new AsyncRelayCommand(CancelSync);
    public ICommand ToggleFollowLogCommand => new RelayCommand(() => FollowLog = !FollowLog);

    private async Task CancelSync()
    {
        if(_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel();
            ReportProgress("Sync cancelled by user.", null, "Cancelled");
        }
    }

    public string Status
    {
        get;
        set => SetProperty(ref field, value);
    } = "Idle";

    [ObservableProperty] private ObservableCollection<DriveItem> _rootItems = [];
    private async Task SignInAsync()
    {
        try
        {
            _logger?.LogInformation("Sign-in started");
            ErrorMessage = string.Empty;
            Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
            // Use Match to handle Result
            if(loginResult is Result<GraphServiceClient, Exception>.Error error)
            {
                ErrorMessage = error.Reason.Message;
                Status = $"Login failed";
                _logger?.LogError(error.Reason, "Sign-in failed");
                return;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            User? me = await client.Me.GetAsync();
            var driveType = client.Me.Drive.GetType().FullName;

            Status = $"Signed in as {me?.DisplayName} - {driveType}";
            _logger?.LogInformation("Sign-in succeeded for {Account}", me?.UserPrincipalName ?? me?.DisplayName ?? "unknown");
        }
        catch(Exception ex)
        {
            Status = $"Login failed: {ex.Message}";
            ErrorMessage = ex.Message;
            _logger?.LogError(ex, "Sign-in failed");
        }
    }
    private async Task SignOutAsync()
    {
        try
        {
            _logger?.LogInformation("Sign-in started");
            ErrorMessage = string.Empty;
            Result<bool, Exception> loginResult = await _loginService.SignOutAsync();
            // Use Match to handle Result
            if(loginResult is Result<bool, Exception>.Error error)
            {
                ErrorMessage = error.Reason.Message;
                Status = $"Login failed";
                _logger?.LogError(error.Reason, "Sign-in failed");
                return;
            }

            Status = $"Signed out";
            _logger?.LogInformation("Sign-out succeeded");
        }
        catch(Exception ex)
        {
            Status = $"Login failed: {ex.Message}";
            ErrorMessage = ex.Message;
            _logger?.LogError(ex, "Sign-in failed");
        }
    }

    public double ProgressValue
    {
        get;
        set => SetProperty(ref field, value);
    }

    private async Task LoadRootItemsAsync()
    {
        try
        {
            _cts = new CancellationTokenSource();
            _logger?.LogInformation("Loading OneDrive root items");
            ErrorMessage = string.Empty;
            await _oneDriveService.GetRootItemsAsync(this, _cts.Token);
        }
        catch(Exception ex)
        {
            Status = $"Load failed: {ex.Message}";
            ErrorMessage = ex is OneDriveService.OneDriveServiceException oneDriveServiceException ? $"OneDrive error ({oneDriveServiceException.StatusCode}): {oneDriveServiceException.Message}" : ex.Message;
            _logger?.LogError(ex, "Failed to load root items {exception}", ex);
        }
    }
}
