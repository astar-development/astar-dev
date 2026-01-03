using System.Collections.ObjectModel;
using System.Windows.Input;
using AStar.Dev.OneDrive.Client.Login;
using AStar.Dev.OneDrive.Client.User;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILogger<MainWindowViewModel> _logger;
    private readonly IOneDriveService _oneDriveService;
    private CancellationTokenSource? _cts = new();
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private ObservableCollection<DriveItem> _rootItems = [];

    public MainWindowViewModel(IOneDriveService oneDriveService, ILogger<MainWindowViewModel> logger)
    {
        _oneDriveService = oneDriveService;
        _logger = logger;
        SignOutCommand = new AsyncRelayCommand(SignOutAsync, () => IsLoggedIn);
        LoadRootCommand = new AsyncRelayCommand(LoadRootItemsAsync, () => !IsSyncing);
        CancelSyncCommand = new AsyncRelayCommand(CancelSync, () => IsSyncing);
        ToggleFollowLogCommand = new RelayCommand(() => UserPreferences.UiSettings.FollowLog = !UserPreferences.UiSettings.FollowLog);

        PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(IsLoggedIn):
                    SignOutCommand.NotifyCanExecuteChanged();
                    break;
                case nameof(IsSyncing):
                    LoadRootCommand.NotifyCanExecuteChanged();
                    CancelSyncCommand.NotifyCanExecuteChanged();
                    break;
            }
        };
    }

    private bool IsSyncing
    {
        get;
        set => SetProperty(ref field, value);
    }

    private bool IsLoggedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<string> ProgressMessages { get; } = [];

    public double ProgressValue
    {
        get;
        set => SetProperty(ref field, value);
    }

    public UserPreferences UserPreferences
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public string Status
    {
        get;
        set => SetProperty(ref field, value);
    } = "No action yet";

    private AsyncRelayCommand SignOutCommand { get; }
    private AsyncRelayCommand LoadRootCommand { get; }
    private AsyncRelayCommand CancelSyncCommand { get; }
    public ICommand ToggleFollowLogCommand { get; }

    internal void ReportProgress(string message, double? progress = null, string? status = null)
        => Dispatcher.UIThread.Post(() =>
        {
            ProgressMessages.Add($"{DateTime.Now:T} - {message}");
            if (progress.HasValue)
                ProgressValue = progress.Value;
            if (!string.IsNullOrEmpty(status))
                Status = status;
        });

    private async Task CancelSync()
    {
        await Task.Delay(1);
        if (IsSyncing && _cts is { IsCancellationRequested: false })
        {
            await _cts.CancelAsync();
            ReportProgress("Sync cancelled by user.", null, "Cancelled");
        }
    }

    private async Task SignOutAsync()
    {
        try
        {
            await Task.Delay(1);
            _logger?.LogInformation("Sign-in started");
            ErrorMessage = string.Empty;
            // Result<bool, Exception> loginResult = await _loginService.SignOutAsync();
            // // Use Match to handle Result
            // if(loginResult is Result<bool, Exception>.Error error)
            // {
            //     ErrorMessage = error.Reason.Message;
            //     Status = $"Login failed";
            //     _logger?.LogError(error.Reason, "Sign-in failed");
            //     return;
            // }

            Status = "Signed out";
            _logger?.LogInformation("Sign-out succeeded");
        }
        catch (Exception ex)
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
            IsSyncing = true;
            IsLoggedIn = true;
            _cts = new CancellationTokenSource();
            _logger.LogInformation("Loading OneDrive root items");
            ErrorMessage = string.Empty;
            await _oneDriveService.RunFullSyncAsync(this, _cts.Token);
        }
        catch (Exception ex)
        {
            Status = $"Load failed: {ex.Message}";
            //ErrorMessage = ex is OneDriveService.OneDriveServiceException oneDriveServiceException ? $"OneDrive error ({oneDriveServiceException.StatusCode}): {oneDriveServiceException.Message}" : ex.Message;
            _logger?.LogError(ex, "Failed to load root items {exception}", ex);
        }
        finally
        {
            IsSyncing = false;
        }
    }
}
