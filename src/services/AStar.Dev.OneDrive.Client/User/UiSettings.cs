namespace AStar.Dev.OneDrive.Client.User;

public class UiSettings
{
    public bool DownloadFilesAfterSync { get; set; }
    public bool RememberMe { get; set; } = true;
    public bool FollowLog { get; set; } = true;
    public string Theme { get; set; } = "Auto";
    public string LastAction { get; set; } = "No action yet";
}
