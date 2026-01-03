namespace AStar.Dev.OneDrive.Client.Scrolling;

public interface IAutoScrollService
{
    bool IsAtOrNearBottom(double offsetY, double extentHeight, double viewportHeight, double tolerance = 1.0);
    bool ShouldScroll(bool autoScrollEnabled, bool followLog, int messageCount);
    double GetBottomOffset(double extentHeight, double viewportHeight);
}
