using Microsoft.Graph;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.OneDrive.Client.Services;

public interface ILoginService
{
    bool IsSignedIn { get; }
        Task<Result<GraphServiceClient, Exception>> SignInAsync();
        Task<Result<bool, Exception>> SignOutAsync();
}
