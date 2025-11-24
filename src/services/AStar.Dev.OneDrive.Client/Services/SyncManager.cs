
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public SyncManager(GraphServiceClient client, DeltaStore store)
    {
        _client = client;
        _store = store;
    }

    public async Task RunSyncAsync()
    {
        Drive? drive = await _client.Me.Drive.GetAsync();
        if(drive?.Id is null)
        {
            Console.WriteLine("No drive found.");
            return;
        }

        var savedToken = await _store.GetDeltaTokenAsync(drive.Id);

        if(string.IsNullOrEmpty(savedToken))
        {
            Console.WriteLine("🔄 First full sync...");
            await FullDeltaSyncAsync(drive.Id);
        }
        else
        {
            Console.WriteLine("🔄 Resuming from saved delta token...");
            await ResumeDeltaSyncAsync(drive.Id, savedToken);
        }
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
            });

        await ProcessDeltaItemsAsync(head?.Value);

        var nextLink = head?.OdataNextLink;
        var finalToken = head?.OdataDeltaLink;

        while(!string.IsNullOrEmpty(nextLink))
        {
            DriveItemCollectionResponse? nextPage = await _client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
                new RequestInformation
                {
                    HttpMethod = Method.GET,
                    UrlTemplate = nextLink
                },
                DriveItemCollectionResponse.CreateFromDiscriminatorValue);

            await ProcessDeltaPageAsync(nextPage);

            nextLink = nextPage?.OdataNextLink;

            var pageToken = TryGetDeltaLink(nextPage);
            if(!string.IsNullOrEmpty(pageToken))
            {
                finalToken = pageToken;
            }
        }

        if(!string.IsNullOrEmpty(finalToken))
        {
            await _store.SaveDeltaTokenAsync(driveId, finalToken);
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

        await ProcessDeltaPageAsync(changes);

        var nextLink = changes?.OdataNextLink;
        var finalToken = TryGetDeltaLink(changes) ?? savedToken;

        while(!string.IsNullOrEmpty(nextLink))
        {
            DriveItemCollectionResponse? nextPage = await _client.RequestAdapter.SendAsync<DriveItemCollectionResponse>(
                new RequestInformation
                {
                    HttpMethod = Method.GET,
                    UrlTemplate = nextLink
                },
                DriveItemCollectionResponse.CreateFromDiscriminatorValue);

            await ProcessDeltaPageAsync(nextPage);

            nextLink = nextPage?.OdataNextLink;

            var pageToken = TryGetDeltaLink(nextPage);
            if(!string.IsNullOrEmpty(pageToken))
            {
                finalToken = pageToken;
            }
        }

        if(!string.IsNullOrEmpty(finalToken))
        {
            await _store.SaveDeltaTokenAsync(driveId, finalToken);
        }
    }

    private async Task ProcessDeltaPageAsync(DriveItemCollectionResponse? page)
    {
        if(page?.Value == null)
            return;
        foreach(DriveItem item in page.Value)
        {
            await HandleItemAsync(item);
        }
    }

    private async Task ProcessDeltaItemsAsync(IList<DriveItem>? items)
    {
        if(items == null)
            return;
        foreach(DriveItem item in items)
        {
            await HandleItemAsync(item);
        }
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

            await _store.SaveItemAsync(
                id: item.Id!,
                name: item.Name!,
                isFolder: isFolder,
                lastModified: item.LastModifiedDateTime
            );
        }
    }

    private static string? TryGetDeltaLink(DriveItemCollectionResponse? page)
    {
        if(page?.AdditionalData == null)
            return null;
        return page.AdditionalData.TryGetValue("@odata.deltaLink", out var tokenObj) ? tokenObj as string ?? tokenObj?.ToString() : null;
    }
}
