using AStar.Dev.OneDrive.Client.ApplicationConfiguration;
using AStar.Dev.OneDrive.Client.ViewModels;
using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item.Delta;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;

namespace AStar.Dev.OneDrive.Client.Login;

public class SyncManager(GraphServiceClient client, DeltaStore store, MainWindowViewModel vm, ApplicationSettings applicationSettings, CancellationToken token)
{
    public async Task RunSyncAsync()
    {
        vm.ReportProgress("Starting sync...", 0, "Syncing");
        Drive? drive = await client.Me.Drive.GetAsync();
        if (drive?.Id is null)
        {
            vm.ReportProgress("No drive found...", 100);
            Console.WriteLine("No drive found.");
            return;
        }

        var savedToken = await store.GetDeltaTokenAsync(drive.Id, token);
        var metrics = new SyncSessionMetrics();
        if (string.IsNullOrEmpty(savedToken))
        {
            vm.ReportProgress("🔄 First full sync...", 10);
            Console.WriteLine("🔄 First full sync...");
            await FullDeltaSyncAsync(drive.Id, metrics);
        }
        else
        {
            vm.ReportProgress("🔄 Resuming from saved delta token...", 10);
            Console.WriteLine("🔄 Resuming from saved delta token...");
            await ResumeDeltaSyncAsync(drive.Id, savedToken, metrics);
        }

        vm.ReportProgress("Sync complete.", 100, "Complete");
    }

    private async Task FullDeltaSyncAsync(string driveId, SyncSessionMetrics metrics)
    {
        DeltaGetResponse? head = await client.Drives[driveId].Items["root"].Delta
            .GetAsDeltaGetResponseAsync(config =>
            {
                config.QueryParameters.Select =
                [
                    "id", "name", "folder", "file", "lastModifiedDateTime", "deleted", "parentReference"
                ];
                config.QueryParameters.Top = applicationSettings.DownloadBatchSize;
            }, token);

        var finalToken = head?.OdataDeltaLink ?? "";

        await ProcessDeltaItemsAsync(head?.Value, driveId, finalToken, metrics);

        var nextLink = head?.OdataNextLink;

        while (!string.IsNullOrEmpty(nextLink))
        {
            DriveItemCollectionResponse? nextPage = await client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
                new RequestInformation
                {
                    HttpMethod = Method.GET,
                    UrlTemplate = nextLink
                },
                DriveItemCollectionResponse.CreateFromDiscriminatorValue);

            await ProcessDeltaPageAsync(nextPage, driveId, finalToken, metrics);

            nextLink = nextPage?.OdataNextLink;

            var pageToken = TryGetDeltaLink(nextPage);
            if (!string.IsNullOrEmpty(pageToken)) finalToken = pageToken;
        }

        await store.SaveDeltaTokenAsync(driveId, finalToken);
    }

    private async Task ResumeDeltaSyncAsync(string driveId, string savedToken, SyncSessionMetrics metrics)
    {
        DriveItemCollectionResponse? changes = await client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
            new RequestInformation
            {
                HttpMethod = Method.GET,
                UrlTemplate = savedToken
            },
            DriveItemCollectionResponse.CreateFromDiscriminatorValue);

        var finalToken = TryGetDeltaLink(changes) ?? savedToken;

        await ProcessDeltaPageAsync(changes, driveId, finalToken, metrics);

        var nextLink = changes?.OdataNextLink;

        while (!string.IsNullOrEmpty(nextLink))
        {
            DriveItemCollectionResponse? nextPage = await client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
                new RequestInformation
                {
                    HttpMethod = Method.GET,
                    UrlTemplate = nextLink
                },
                DriveItemCollectionResponse.CreateFromDiscriminatorValue);
            vm.ReportProgress($"📊 Process Delta Page: {nextPage?.OdataNextLink ?? "Unknown"}");
            await ProcessDeltaPageAsync(nextPage, driveId, finalToken, metrics);

            nextLink = nextPage?.OdataNextLink;

            var pageToken = TryGetDeltaLink(nextPage);
            if (!string.IsNullOrEmpty(pageToken)) finalToken = pageToken;
        }

        await store.SaveDeltaTokenAsync(driveId, finalToken);
    }

    private async Task ProcessDeltaPageAsync(
        DriveItemCollectionResponse? page,
        string driveId,
        string deltaToken,
        SyncSessionMetrics metrics)
    {
        if (page?.Value == null)
            return;
        await ProcessDeltaItemsAsync(page.Value, driveId, deltaToken, metrics);
    }

    private async Task ProcessDeltaItemsAsync(
        IList<DriveItem>? items,
        string driveId,
        string deltaToken,
        SyncSessionMetrics metrics)
    {
        if (items == null || items.Count == 0)
            return;

        var batch = new List<LocalDriveItem>();
        var deletes = new List<string>();

        foreach (DriveItem graphItem in items)
            if (graphItem.Deleted != null)
                deletes.Add(graphItem.Id!);
            else
                batch.Add(new LocalDriveItem
                {
                    Id = graphItem.Id!, // GUID
                    PathId = graphItem.ParentReference?.Path + "/" + graphItem.Name, // path-based
                    Name = graphItem.Name,
                    IsFolder = graphItem.Folder != null,
                    LastModifiedUtc = graphItem.LastModifiedDateTime?.UtcDateTime.ToString("o"),
                    ParentPath = graphItem.ParentReference?.Path,
                    ETag = graphItem.ETag
                });

        SyncResult result = await store.SaveBatchWithTokenAsync(batch, deletes, driveId, deltaToken, DateTime.UtcNow, token);
        metrics.AddBatch(result.Inserted, result.Updated, result.Deleted);

        vm.ReportProgress(
            $"📊 Sync metrics: {result.Inserted} inserted, {result.Updated} updated, {result.Deleted} deleted. " +
            $"Session totals: {metrics.InsertedTotal} inserted, {metrics.UpdatedTotal} updated, {metrics.DeletedTotal} deleted. " +
            $"Total written={metrics.TotalWritten}. Token={result.DeltaToken}");
    }

    private static string? TryGetDeltaLink(DriveItemCollectionResponse? page) => page?.AdditionalData == null
        ? null
        : page.AdditionalData.TryGetValue("@odata.deltaLink", out var tokenObj)
            ? tokenObj as string ?? tokenObj?.ToString()
            : null;
}
