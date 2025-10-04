using AStar.Dev.Database.Updater.FileKeywordProcessor;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class ThroughputHealthCheckTests
{
    [Fact]
    public async Task Healthy_WhenEnoughEventsInWindow()
    {
        var tracker = new ThroughputTracker();

        for(var i = 0; i < 15; i++)
        {
            tracker.RecordEvent();
        }

        var check = new ThroughputHealthCheck(tracker, 10, TimeSpan.FromMinutes(1));

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Degraded_WhenTooFewEventsInWindow()
    {
        var tracker = new ThroughputTracker();
        tracker.RecordEvent(); // only 1 event

        var check = new ThroughputHealthCheck(tracker, 10, TimeSpan.FromMinutes(1));

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public async Task OldEvents_AreDiscardedFromWindow()
    {
        var tracker = new ThroughputTracker();

        // Simulate an old event
        tracker.RecordEvent();

        // Manually adjust queue to simulate old timestamp
        // (In real tests you might inject a clock abstraction for cleaner control)

        var check = new ThroughputHealthCheck(tracker, 1, TimeSpan.Zero);

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Degraded);
    }
}
