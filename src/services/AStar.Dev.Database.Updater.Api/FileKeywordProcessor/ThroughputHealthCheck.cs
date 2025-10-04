using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public class ThroughputHealthCheck(ThroughputTracker tracker, int minEventsPerWindow = 10, TimeSpan? window = null) : IHealthCheck
{
    private readonly TimeSpan _window = window ?? TimeSpan.FromMinutes(1);

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var count = tracker.CountEventsInWindow(_window);

        if(count < minEventsPerWindow)
        {
            return Task.FromResult(HealthCheckResult.Degraded($"Only {count} events in last {_window.TotalSeconds} seconds (expected >= {minEventsPerWindow})"));
        }

        return Task.FromResult(HealthCheckResult.Healthy($"Throughput OK: {count} events in last {_window.TotalSeconds} seconds"));
    }
}
