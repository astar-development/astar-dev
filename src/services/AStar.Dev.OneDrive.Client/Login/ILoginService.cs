using AStar.Dev.Functional.Extensions;
using Microsoft.Graph;

namespace AStar.Dev.OneDrive.Client.Login;

public interface ILoginService
{
    Task<Result<GraphServiceClient, Exception>> CreateGraphServiceClientAsync();
    Task<Result<bool, Exception>> SignOutAsync(bool hard = false);
}
