using System.Threading.Channels;
using AStar.Dev.Database.Updater.Core.FileKeywordProcessor;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class ChannelBacklogHealthCheckTests
{
    private static TrackedChannel<FileKeywordMatch> CreateTrackedChannel()
    {
        var inner = Channel.CreateUnbounded<FileKeywordMatch>(
                                                              new() { SingleReader = true, SingleWriter = false });

        return new(inner);
    }

    [Fact]
    public async Task Healthy_WhenBacklogIsLow()
    {
        var tracked = CreateTrackedChannel();
        var check   = new ChannelBacklogHealthCheck(tracked, 5, 10);

        // No writes yet, backlog = 0
        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Degraded_WhenBacklogExceedsWarning()
    {
        var tracked = CreateTrackedChannel();
        var check   = new ChannelBacklogHealthCheck(tracked, 5, 10);

        // Write 6 items (backlog = 6)
        for(var i = 0; i < 6; i++)
        {
            tracked.Writer.TryWrite(new() { FileName = "f", Keyword = "k" });
        }

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public async Task Unhealthy_WhenBacklogExceedsCritical()
    {
        var tracked = CreateTrackedChannel();
        var check   = new ChannelBacklogHealthCheck(tracked, 5, 10);

        // Write 11 items (backlog = 11)
        for(var i = 0; i < 11; i++)
        {
            tracked.Writer.TryWrite(new() { FileName = "f", Keyword = "k" });
        }

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
    }

    [Fact]
    public async Task Backlog_Decreases_WhenItemsAreRead()
    {
        var tracked = CreateTrackedChannel();
        var check   = new ChannelBacklogHealthCheck(tracked, 5, 10);

        // Write 3 items
        for(var i = 0; i < 3; i++)
        {
            tracked.Writer.TryWrite(new() { FileName = "f", Keyword = "k" });
        }

        tracked.Count.ShouldBe(3);

        // Read one item
        var _ = await tracked.Reader.ReadAsync();

        tracked.Count.ShouldBe(2);

        var result = await check.CheckHealthAsync(new());
        result.Status.ShouldBe(HealthStatus.Healthy);
    }
}
