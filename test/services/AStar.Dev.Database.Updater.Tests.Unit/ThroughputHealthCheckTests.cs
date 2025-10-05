using AStar.Dev.Database.Updater.Core.FileKeywordProcessor;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Time.Testing;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class ThroughputHealthCheckTests
{
    [Fact]
    public async Task Healthy_WhenEnoughEventsInWindow()
    {
        var fakeTime = new FakeTimeProvider();
        var tracker  = new ThroughputTracker(fakeTime);

        // Record 15 events at time 0
        for(var i = 0; i < 15; i++)
        {
            tracker.RecordEvent();
        }

        var check = new ThroughputHealthCheck(tracker, TimeSpan.FromMinutes(1));

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Degraded_WhenTooFewEventsInWindow()
    {
        var fakeTime = new FakeTimeProvider();
        var tracker  = new ThroughputTracker(fakeTime);

        tracker.RecordEvent(); // only 1 event

        var check = new ThroughputHealthCheck(tracker, TimeSpan.FromMinutes(1));

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public async Task OldEvents_AreDiscardedFromWindow()
    {
        var fakeTime = new FakeTimeProvider();
        var tracker  = new ThroughputTracker(fakeTime);

        tracker.RecordEvent(); // event at time 0

        // Advance time beyond the window
        fakeTime.Advance(TimeSpan.FromMinutes(2));

        var check = new ThroughputHealthCheck(tracker, TimeSpan.FromMinutes(1), 1);

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Degraded);
    }
}
