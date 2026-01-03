using AStar.Dev.OneDrive.Client.Scrolling;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Scrolling;

public class AutoScrollServiceShould
{
    private readonly AutoScrollService _sut = new();

    [Fact]
    public void AutoScrollServiceShouldDetectBeingAtOrNearBottomWithTolerance()
    {
        // offset is just within tolerance from the bottom
        const double extent = 100.0;
        const double viewport = 20.0;
        var offset = extent - viewport - 0.9; // 0.9 within default tolerance 1.0

        _sut.IsAtOrNearBottom(offset, extent, viewport).ShouldBeTrue();
    }

    [Fact]
    public void AutoScrollServiceShouldReturnFalseWhenFarFromBottom()
    {
        const double extent = 100.0;
        const double viewport = 20.0;
        const double offset = 50.0; // far from the bottom (bottom would be 80)

        _sut.IsAtOrNearBottom(offset, extent, viewport).ShouldBeFalse();
    }

    [Fact]
    public void AutoScrollServiceShouldReturnTrueWhenContentFitsOrNearlyFitsViewport()
    {
        // Content smaller than viewport
        _sut.IsAtOrNearBottom(0, 10, 20).ShouldBeTrue();

        // Content equal to viewport within tolerance
        _sut.IsAtOrNearBottom(0, 20.5, 20).ShouldBeTrue();
    }

    [Fact]
    public void AutoScrollServiceShouldDetermineWhenToScrollBasedOnFlagsAndMessageCount()
    {
        // Only scroll when auto-scroll enabled, follow log enabled, and at least one message
        _sut.ShouldScroll(true, true, 1).ShouldBeTrue();

        _sut.ShouldScroll(false, true, 1).ShouldBeFalse();
        _sut.ShouldScroll(true, false, 1).ShouldBeFalse();
        _sut.ShouldScroll(true, true, 0).ShouldBeFalse();
        _sut.ShouldScroll(false, false, 0).ShouldBeFalse();
    }

    [Fact]
    public void AutoScrollServiceShouldCalculateBottomOffsetClampedAtZero()
    {
        // Normal case: extent greater than viewport
        _sut.GetBottomOffset(100, 20).ShouldBe(80);

        // Equal sizes: bottom is zero
        _sut.GetBottomOffset(50, 50).ShouldBe(0);

        // Viewport larger than content: clamp to zero
        _sut.GetBottomOffset(10, 50).ShouldBe(0);
    }
}
