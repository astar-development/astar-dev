
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AStar.Dev.OneDrive.Client.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.Graph;
using Microsoft.Graph.Drives.Item.Items.Item.Delta;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;

namespace AStar.Dev.OneDrive.Client.Services;

public class SyncManager
{
    private readonly GraphServiceClient _client;
    private readonly DeltaStore _store;
    private readonly MainWindowViewModel _vm;
    private readonly CancellationToken _token;

    public SyncManager(GraphServiceClient client, DeltaStore store, MainWindowViewModel vm, CancellationToken token)
    {
        _client = client;
        _store = store;
        _vm = vm;
        _token = token;
    }

    public async Task RunSyncAsync()
    {
        _vm.ReportProgress("Starting sync...", 0, "Syncing");
        Drive? drive = await _client.Me.Drive.GetAsync();
        if(drive?.Id is null)
        {
            _vm.ReportProgress("No drive found...", 100);
            Console.WriteLine("No drive found.");
            return;
        }

        var savedToken = await _store.GetDeltaTokenAsync(drive.Id, _token);

        if(string.IsNullOrEmpty(savedToken))
        {
            _vm.ReportProgress("🔄 First full sync...", 10);
            Console.WriteLine("🔄 First full sync...");
            await FullDeltaSyncAsync(drive.Id);
        }
        else
        {
            _vm.ReportProgress("🔄 Resuming from saved delta token...", 10);
            Console.WriteLine("🔄 Resuming from saved delta token...");
            await ResumeDeltaSyncAsync(drive.Id, savedToken);
        }

        _vm.ReportProgress("Sync complete.", 100, "Complete");
    }

    private async Task FullDeltaSyncAsync(string driveId)
    {
        DeltaGetResponse? head = await _client.Drives[driveId].Items["root"].Delta
        .GetAsDeltaGetResponseAsync(config =>
        {
            config.QueryParameters.Select = new[]
            {
                "id","name","folder","file","lastModifiedDateTime","deleted","parentReference"
            };
            config.QueryParameters.Top = 200;
        }, _token);

        var finalToken = head?.OdataDeltaLink ?? "";

        await ProcessDeltaItemsAsync(head?.Value, driveId, finalToken);

        var nextLink = head?.OdataNextLink;

        while(!string.IsNullOrEmpty(nextLink))
        {
            DriveItemCollectionResponse? nextPage = await _client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
            new RequestInformation
            {
                HttpMethod = Method.GET,
                UrlTemplate = nextLink
            },
            DriveItemCollectionResponse.CreateFromDiscriminatorValue);

            await ProcessDeltaPageAsync(nextPage, driveId, finalToken);

            nextLink = nextPage?.OdataNextLink;

            var pageToken = TryGetDeltaLink(nextPage);
            if(!string.IsNullOrEmpty(pageToken))
            {
                finalToken = pageToken;
            }
        }
    }
    private async Task ResumeDeltaSyncAsync(string driveId, string savedToken)
    {
        DriveItemCollectionResponse? changes = await _client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
        new RequestInformation
        {
            HttpMethod = Method.GET,
            UrlTemplate = savedToken
        },
        DriveItemCollectionResponse.CreateFromDiscriminatorValue);

        var finalToken = TryGetDeltaLink(changes) ?? savedToken;

        await ProcessDeltaPageAsync(changes, driveId, finalToken);

        var nextLink = changes?.OdataNextLink;

        while(!string.IsNullOrEmpty(nextLink))
        {
            DriveItemCollectionResponse? nextPage = await _client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
            new RequestInformation
            {
                HttpMethod = Method.GET,
                UrlTemplate = nextLink
            },
            DriveItemCollectionResponse.CreateFromDiscriminatorValue);
            _vm.ReportProgress($"📊 Process Delta Page: {nextPage?.OdataNextLink ?? "Unknown"}");
            await ProcessDeltaPageAsync(nextPage, driveId, finalToken);

            nextLink = nextPage?.OdataNextLink;

            var pageToken = TryGetDeltaLink(nextPage);
            if(!string.IsNullOrEmpty(pageToken))
            {
                finalToken = pageToken;
            }
        }
    }

    private async Task ProcessDeltaPageAsync(
    DriveItemCollectionResponse? page,
    string driveId,
    string deltaToken)
    {
        if(page?.Value == null)
            return;
        await ProcessDeltaItemsAsync(page.Value, driveId, deltaToken);
    }

    private async Task ProcessDeltaItemsAsync(
     IList<DriveItem>? items,
     string driveId,
     string deltaToken)
    {
        if(items == null || items.Count == 0)
            return;

        var batch = new List<LocalDriveItem>();
        var deletes = new List<string>();

        foreach(DriveItem item in items)
        {
            if(item.Deleted != null)
            {
                deletes.Add(item.Id!);
            }
            else
            {
                batch.Add(new LocalDriveItem
                {
                    Id = item.Id!,
                    Name = item.Name,
                    IsFolder = item.Folder != null,
                    LastModifiedUtc = item.LastModifiedDateTime?.UtcDateTime.ToString("o"),
                    ParentPath = item.ParentReference?.Path,
                    ETag = item.ETag
                });
            }
        }

        SyncResult result = await _store.SaveBatchWithTokenAsync(batch, deletes, driveId, deltaToken, DateTime.UtcNow, _token);

        Console.WriteLine($"📊 Sync metrics: {result.Inserted} inserted, {result.Updated} updated, {result.Deleted} deleted. Token={result.DeltaToken}");
        _vm.ReportProgress($"📊 Sync metrics: {result.Inserted} inserted, {result.Updated} updated, {result.Deleted} deleted. Token={result.DeltaToken}");
    }

    private async Task HandleItemAsync(DriveItem item)
    {
        if(item.Deleted != null)
        {
            Console.WriteLine($"❌ Deleted: {item.Id}");
            // TODO: remove from local index
        }
        else
        {
            var isFolder = item.Folder != null;
            var parentPath = item.ParentReference?.Path;
            Console.WriteLine($"✅ {item.Name} ({(isFolder ? "Folder" : "File")}) - {parentPath}");
            _vm.ReportProgress($"✅ {item.Name} ({(isFolder ? "Folder" : "File")}) - {parentPath}");

            await _store.SaveItemAsync(
                id: item.Id!,
                name: item.Name!,
                isFolder: isFolder,
                lastModified: item.LastModifiedDateTime,
                parentPath: parentPath,
                eTag: item.ETag
            );
        }
    }

    private static string? TryGetDeltaLink(DriveItemCollectionResponse? page) => page?.AdditionalData == null
            ? null
            : page.AdditionalData.TryGetValue("@odata.deltaLink", out var tokenObj) ? tokenObj as string ?? tokenObj?.ToString() : null;
}
