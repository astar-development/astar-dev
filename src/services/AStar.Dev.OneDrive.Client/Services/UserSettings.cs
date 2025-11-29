namespace AStar.Dev.OneDrive.Client.Services;

public sealed class UserSettings
{
    public string Theme { get; set; } = "Auto"; // Auto | Light | Dark
    public double WindowWidth { get; set; } = 800;
    public double WindowHeight { get; set; } = 600;
    public int? WindowX { get; set; }
    public int? WindowY { get; set; }
    public string? LastAction { get; set; }

    // new property
    public bool DownloadFilesAfterSync { get; set; } = false;
    public bool RememberMe { get; set; } = true; // default to true

    // New: configurable max parallel downloads
    public int MaxParallelDownloads { get; set; } = 2;
    // New: configurable batch size for DB updates
    public int DownloadBatchSize { get; set; } = 100;
    public int CacheTag { get; set; } = 1; // used to version the cache name
    public bool FollowLog { get; internal set; }

}
