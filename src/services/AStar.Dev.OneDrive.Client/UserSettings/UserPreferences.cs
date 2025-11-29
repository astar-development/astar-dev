namespace AStar.Dev.OneDrive.Client.UserSettings;

public sealed class UserPreferences
{
    public string Theme { get; set; } = "Auto"; // Auto | Light | Dark

    public double WindowWidth { get; set; } = 800;

    public double WindowHeight { get; set; } = 600;

    public int? WindowX { get; set; }

    public int? WindowY { get; set; }

    public string? LastAction { get; set; }

    public bool DownloadFilesAfterSync { get; set; } = false;

    public bool RememberMe { get; set; } = true; // default to true

    public int MaxParallelDownloads { get; set; } = 2;

    public int DownloadBatchSize { get; set; } = 100;

    public int CacheTag { get; set; } = 1; // used to version the cache name

    public bool FollowLog { get; internal set; }
}
