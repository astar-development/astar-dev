namespace AStar.Dev.OneDrive.Client.Login;

public class SyncAggregate
{
    public int TotalInserted { get; set; }
    public int TotalUpdated { get; set; }
    public int TotalDeleted { get; set; }
    public int RunCount { get; set; }
    public double AvgInserted => RunCount == 0 ? 0 : (double)TotalInserted / RunCount;
    public double AvgUpdated => RunCount == 0 ? 0 : (double)TotalUpdated / RunCount;
    public double AvgDeleted => RunCount == 0 ? 0 : (double)TotalDeleted / RunCount;
}
