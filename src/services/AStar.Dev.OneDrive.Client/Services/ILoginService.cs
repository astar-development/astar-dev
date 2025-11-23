using Microsoft.Graph;

namespace AStar.Dev.OneDrive.Client.Services;

public interface ILoginService
{
    bool IsSignedIn { get; }
    Task<GraphServiceClient> SignInAsync();
    Task SignOutAsync();
}
