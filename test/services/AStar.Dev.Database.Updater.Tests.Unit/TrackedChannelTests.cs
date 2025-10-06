using System.Threading.Channels;
using AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

namespace AStar.Dev.Database.Updater.Tests.Unit;

public class TrackedChannelTests
{
    [Fact]
    public void TryRead_ReturnsFalse_WhenEmpty_SetsItemToDefault()
    {
        var inner   = Channel.CreateUnbounded<string>();
        var tracked = new TrackedChannel<string>(inner);

        tracked.Reader.TryRead(out var item).ShouldBeFalse();
        item.ShouldBeNull(); // default for string
    }

    [Fact]
    public void TryPeek_ReturnsFalse_WhenEmpty_SetsItemToDefault()
    {
        var inner   = Channel.CreateUnbounded<string>();
        var tracked = new TrackedChannel<string>(inner);

        tracked.Reader.TryPeek(out var item).ShouldBeFalse();
        item.ShouldBeNull(); // default for string
    }

    [Fact]
    public void TryRead_ReturnsTrue_WhenItemAvailable_DecrementsCount()
    {
        var inner   = Channel.CreateUnbounded<string>();
        var tracked = new TrackedChannel<string>(inner);

        tracked.Writer.TryWrite("hello").ShouldBeTrue();
        tracked.Count.ShouldBe(1);

        tracked.Reader.TryRead(out var item).ShouldBeTrue();
        item.ShouldBe("hello");
        tracked.Count.ShouldBe(0);
    }

    [Fact]
    public void TryPeek_ReturnsTrue_WhenItemAvailable_DoesNotDecrementCount()
    {
        var inner   = Channel.CreateUnbounded<string>();
        var tracked = new TrackedChannel<string>(inner);

        tracked.Writer.TryWrite("peek");
        tracked.Count.ShouldBe(1);

        tracked.Reader.TryPeek(out var item).ShouldBeTrue();
        item.ShouldBe("peek");
        tracked.Count.ShouldBe(1); // still 1, peek does not consume
    }
}
