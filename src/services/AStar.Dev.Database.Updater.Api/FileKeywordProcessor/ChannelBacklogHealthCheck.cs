using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public class ChannelBacklogHealthCheck(ChannelBacklogTracker tracker, int warningThreshold = 10_000, int criticalThreshold = 50_000) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var count = tracker.Count;

        if(count > criticalThreshold)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Channel backlog too high: {count}"));
        }

        if(count > warningThreshold)
        {
            return Task.FromResult(HealthCheckResult.Degraded($"Channel backlog growing: {count}"));
        }

        return Task.FromResult(HealthCheckResult.Healthy($"Channel backlog is {count}"));
    }
}
