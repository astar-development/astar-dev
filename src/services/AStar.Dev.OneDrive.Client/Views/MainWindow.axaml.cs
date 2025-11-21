using AStar.Dev.OneDrive.Client.Services;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Azure.Identity;
using Microsoft.Graph;

namespace AStar.Dev.OneDrive.Client.Views;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    private void InitializeComponent()
    {
        
        var settings = new AppSettings { ClientId = "09627007-a4e9-4f3c-aed0-175b36552d8f", TenantId = "bb7d94aa-36a9-4a59-a0c1-54a757c47ddf" };
        var options = new InteractiveBrowserCredentialOptions { ClientId = settings.ClientId, TenantId = settings.TenantId, RedirectUri = new Uri("http://localhost") };

        var credential = new InteractiveBrowserCredential(options);

        string[] scopes = { "User.Read", "Files.ReadWrite.All", "offline_access" };

        var client = new GraphServiceClient(credential, scopes);
        
        AvaloniaXamlLoader.Load(this);
        DataContext = new MainWindowViewModel(new LoginService(settings), new OneDriveService(client));
    }
}
