namespace AStar.Dev.OneDrive.Client.Login;

public class SyncSessionMetrics
{
    public int InsertedTotal { get; private set; }
    public int UpdatedTotal { get; private set; }
    public int DeletedTotal { get; private set; }

    public int TotalWritten => InsertedTotal + UpdatedTotal + DeletedTotal;

    public void AddBatch(int inserted, int updated, int deleted)
    {
        InsertedTotal += inserted;
        UpdatedTotal += updated;
        DeletedTotal += deleted;
    }
}
