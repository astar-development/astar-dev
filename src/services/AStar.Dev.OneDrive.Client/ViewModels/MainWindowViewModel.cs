using AStar.Dev.OneDrive.Client;
using AStar.Dev.OneDrive.Client.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Graph;
using Microsoft.Graph.Models;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly ILoginService _loginService;
    private readonly OneDriveService _oneDriveService;

    [ObservableProperty] private string _status = "Not signed in";

    public MainWindowViewModel(ILoginService loginService, OneDriveService oneDriveService) 
    {
        _loginService = loginService;
        _oneDriveService = oneDriveService;
        SignInCommand = new AsyncRelayCommand(SignInAsync);
    }

    public IAsyncRelayCommand SignInCommand { get; }

    private async Task SignInAsync()
    {
        try
        {
            GraphServiceClient client = await _loginService.SignInAsync();

            User? me = await client.Me.GetAsync();
            var driveType = client.Me.Drive.GetType().FullName;

            Status = $"Signed in as {me?.DisplayName} - {driveType}";
            List<DriveItem> x = await _oneDriveService.GetRootItemsAsync();
            
            foreach (DriveItem item in x) Console.WriteLine(item.Name);
        }
        catch(Exception ex)
        {
            Status = $"Login failed: {ex.Message}";
        }
    }
}
