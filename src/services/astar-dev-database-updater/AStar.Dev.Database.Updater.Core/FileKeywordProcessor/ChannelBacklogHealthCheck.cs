using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
public class ChannelBacklogHealthCheck : IHealthCheck
{
    private readonly TrackedChannel<FileKeywordMatch> _channel;
    private readonly int                              _criticalThreshold;
    private readonly int                              _warningThreshold;

    /// <summary>
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="warningThreshold"></param>
    /// <param name="criticalThreshold"></param>
    public ChannelBacklogHealthCheck(TrackedChannel<FileKeywordMatch> channel,
                                     int                              warningThreshold  = 10_000,
                                     int                              criticalThreshold = 50_000)
    {
        _channel           = channel;
        _warningThreshold  = warningThreshold;
        _criticalThreshold = criticalThreshold;
    }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken  cancellationToken = default)
    {
        var count = _channel.Count;

        if(count > _criticalThreshold)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy($"Channel backlog too high: {count}"));
        }

        if(count > _warningThreshold)
        {
            return Task.FromResult(HealthCheckResult.Degraded($"Channel backlog growing: {count}"));
        }

        return Task.FromResult(HealthCheckResult.Healthy($"Channel backlog is {count}"));
    }
}
