namespace AStar.Dev.OneDrive.Client.ApplicationConfiguration;

public class ApplicationSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public int MaxParallelDownloads { get; set; } = 2;
    public int DownloadBatchSize { get; set; } = 100;
    public int CacheTag { get; set; } = 1;
    public string ApplicationVersion { get; set; } = "1.0.0";
    public string UserPreferencesPath { get; set; } = "/home/jason/Documents/.config/astar-dev/AStar.OneDrive.Client.settings.json";
    public string[] Scopes { get; set; } = [];
}
