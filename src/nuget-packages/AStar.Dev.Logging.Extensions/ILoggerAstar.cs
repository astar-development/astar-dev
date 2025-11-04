using Microsoft.Extensions.Logging;

namespace AStar.Dev.Logging.Extensions;

/// <summary>
/// </summary>
/// <typeparam name="T">The type of the parameter</typeparam>
public interface ILoggerAstar<out T> : ILogger<T>
{
    /// <summary>
    /// </summary>
    /// <param name="pageName"></param>
    void LogPageView(string pageName);
}
