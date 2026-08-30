using Microsoft.Extensions.Logging;

namespace Fab4Kids.Web.Logging;

/// <summary>
///     App-local compatibility shim providing the small logging call-site surface the app code expects,
///     wrapping the standard <see cref="ILogger" /> methods.
/// </summary>
internal static class LogMessage
{
    public static void Error(ILogger logger, string message) => logger.LogError("{Message}", message);

    public static void Warning(ILogger logger, string eventName, object? data = null) => logger.LogWarning("{EventName} {Data}", eventName, data);

    public static void Information(ILogger logger, string eventName, object? data = null) => logger.LogInformation("{EventName} {Data}", eventName, data);

    public static void Information(ILogger logger, string eventName, object? data1, object? data2) => logger.LogInformation("{EventName} {Data1} {Data2}", eventName, data1, data2);

    public static void Debug(ILogger logger, string eventName, object? data = null) => logger.LogDebug("{EventName} {Data}", eventName, data);

    public static void NotFound(ILogger logger, string path) => logger.LogWarning("not-found {Path}", path);

    public static void LogException(ILogger logger, string source, string exceptionType, string exceptionMessage, string stackTrace) =>
        logger.LogError("{Source} threw {ExceptionType}: {ExceptionMessage}{NewLine}{StackTrace}", source, exceptionType, exceptionMessage, Environment.NewLine, stackTrace);
}
