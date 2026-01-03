namespace AStar.Dev.OneDrive.Client.Login;

public class SyncResult
{
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Deleted { get; set; }
    public string DeltaToken { get; set; } = "";
    public DateTime LastSyncedUtc { get; set; }
}
