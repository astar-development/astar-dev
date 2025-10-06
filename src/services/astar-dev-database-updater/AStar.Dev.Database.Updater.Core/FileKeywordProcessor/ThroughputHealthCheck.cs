using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
///     Health check that reports degraded if throughput falls below a threshold.
/// </summary>
public class ThroughputHealthCheck(
    ThroughputTracker tracker,
    TimeSpan          window,
    int               minEventsPerWindow = 10) : IHealthCheck
{
    private readonly int               _minEventsPerWindow = minEventsPerWindow;
    private readonly ThroughputTracker _tracker            = tracker;
    private readonly TimeSpan          _window             = window;

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken  cancellationToken = default)
    {
        var count = _tracker.CountEventsInWindow(_window);

        if(count < _minEventsPerWindow)
        {
            return Task.FromResult(HealthCheckResult.Degraded($"Only {count} events in last {_window.TotalSeconds} seconds (expected >= {_minEventsPerWindow})"));
        }

        return Task.FromResult(HealthCheckResult.Healthy($"Throughput OK: {count} events in last {_window.TotalSeconds} seconds"));
    }
}
