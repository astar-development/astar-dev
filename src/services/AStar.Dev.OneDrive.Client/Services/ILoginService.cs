using AStar.Dev.Functional.Extensions;
using Microsoft.Graph;

namespace AStar.Dev.OneDrive.Client.Services;

public interface ILoginService
{
    bool IsSignedIn { get; }

    Task<Result<GraphServiceClient, Exception>> SignInAsync();
    Task<Result<bool, Exception>> SignOutAsync(bool hard = false);
}
