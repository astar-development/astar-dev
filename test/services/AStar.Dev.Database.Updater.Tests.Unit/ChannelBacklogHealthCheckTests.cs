using System.Threading.Channels;
using AStar.Dev.Database.Updater.FileKeywordProcessor;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class ChannelBacklogHealthCheckTests
{
    [Fact]
    public async Task Healthy_WhenBacklogIsLow()
    {
        var channel = Channel.CreateUnbounded<FileKeywordMatch>();
        var check   = new ChannelBacklogHealthCheck(channel.Reader, 5, 10);

        // No items in channel
        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Healthy);
    }

    [Fact]
    public async Task Degraded_WhenBacklogExceedsWarning()
    {
        var channel = Channel.CreateUnbounded<FileKeywordMatch>();

        for(var i = 0; i < 6; i++)
        {
            channel.Writer.TryWrite(new() { FileName = "f", Keyword = "k" });
        }

        var check = new ChannelBacklogHealthCheck(channel.Reader, 5, 10);

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Degraded);
    }

    [Fact]
    public async Task Unhealthy_WhenBacklogExceedsCritical()
    {
        var channel = Channel.CreateUnbounded<FileKeywordMatch>();

        for(var i = 0; i < 11; i++)
        {
            channel.Writer.TryWrite(new() { FileName = "f", Keyword = "k" });
        }

        var check = new ChannelBacklogHealthCheck(channel.Reader, 5, 10);

        var result = await check.CheckHealthAsync(new());

        result.Status.ShouldBe(HealthStatus.Unhealthy);
    }
}
