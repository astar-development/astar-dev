using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Logging.Extensions;

/// <summary>
/// </summary>
/// <param name="logger"></param>
/// <param name="telemetryClient"></param>
/// <typeparam name="TCategoryName"></typeparam>
public sealed class AStarLogger<TCategoryName>(ILogger<TCategoryName> logger, TelemetryClient telemetryClient) : ILoggerAstar<TCategoryName>
{
    /// <inheritdoc />
    public void LogPageView(string pageName)
    {
        logger.LogInformation(AStarEventIds.PageView, "Page view: {PageView}", pageName);

        telemetryClient.TrackPageView(pageName);
    }

    /// <inheritdoc />
    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull =>
        logger.BeginScope(state);

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) =>
        logger.IsEnabled(logLevel);

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        logger.Log(logLevel, eventId, state, exception, formatter);
}
