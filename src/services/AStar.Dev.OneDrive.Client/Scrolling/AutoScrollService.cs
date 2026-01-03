namespace AStar.Dev.OneDrive.Client.Scrolling;

public class AutoScrollService : IAutoScrollService
{
    public bool IsAtOrNearBottom(double offsetY, double extentHeight, double viewportHeight, double tolerance = 1.0)
        => extentHeight <= viewportHeight + tolerance || offsetY >= extentHeight - viewportHeight - tolerance;

    public bool ShouldScroll(bool autoScrollEnabled, bool followLog, int messageCount)
        => autoScrollEnabled && followLog && messageCount > 0;

    public double GetBottomOffset(double extentHeight, double viewportHeight)
        => Math.Max(0, extentHeight - viewportHeight);
}
