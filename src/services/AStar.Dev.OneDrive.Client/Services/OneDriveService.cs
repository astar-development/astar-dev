using System.Collections.Concurrent;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.OneDrive.Client.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class OneDriveService(ILoginService loginService, UserSettings userSettings, ILogger<OneDriveService> logger)
{
    private DeltaStore _store = null!;
    private GraphServiceClient _client = null!;

    // Custom exception type for higher-level error surface
    public sealed class OneDriveServiceException(string message, int? statusCode = null, Exception? inner = null) : Exception(message, inner)
    {
        public int? StatusCode { get; } = statusCode;
    }
    public async Task RunFullSyncAsync(MainWindowViewModel vm, CancellationToken token)
    {
        // 1. Sign in
        Result<GraphServiceClient, Exception> loginResult = await loginService.CreateGraphServiceClientAsync();
        if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            throw loginErr.Reason;

        _client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;

        // 2. Init DB
        var appDataPath = AppPathHelper.GetAppDataPath("astar-dev");
        var fullAppDataPath = Path.Combine(appDataPath, "astar-dev-onedrive-client", "onedrive-sync");
        _ = Directory.CreateDirectory(fullAppDataPath);

        var dbPath = Path.Combine(fullAppDataPath, "onedrive_sync.db");
        _store = new DeltaStore(logger, dbPath);

var SyncManager = new SyncManager(_client, _store, vm, token);
        await SyncManager.RunSyncAsync();

        if(userSettings.DownloadFilesAfterSync)
            await DownloadFilesAsync(vm, userSettings, token);
    }

    private async Task BootstrapDriveAsync(string driveId, MainWindowViewModel vm, CancellationToken token)
    {
        // Fetch root item from Graph (v5 Kiota)
        DriveItem? rootItem = await _client.Drives[driveId].Items["root"]
        .GetAsync(cancellationToken: token);

        if(rootItem is null)
        {
            vm.ReportProgress("⚠️ Unable to load root item.");
            return;
        }

        vm.ReportProgress($"ℹ️ Inserting Root ({rootItem.Name}) into dB...");
        // Keep signature: DeltaStore maps DriveItem -> LocalDriveItem and persists metadata
        await _store.InsertRootAsync(driveId, rootItem, token);

        // Recursively insert children
        await InsertChildrenRecursiveAsync(driveId, rootItem.Id!, $"/drives/{driveId}/root:", vm, token);
    }

    private async Task InsertChildrenRecursiveAsync(
        string driveId,
        string itemId,
        string parentPath,
        MainWindowViewModel vm,
        CancellationToken token)
    {
        vm.ReportProgress($"ℹ️ Getting children for {parentPath}...");

        DriveItemCollectionResponse? childrenResponse =
        await _client.Drives[driveId].Items[itemId].Children
            .GetAsync(cancellationToken: token);

        List<DriveItem>? children = childrenResponse?.Value;
        if(children is null || children.Count == 0)
            return;

        vm.ReportProgress($"ℹ️ Inserting {children.Count} children for {parentPath} into dB...");
        // Keep signature: pass raw DriveItem list; DeltaStore performs the mapping and metadata persistence
        await _store.InsertChildrenAsync(parentPath, children, token);

        // Recurse into subfolders
        foreach(DriveItem child in children)
        {
            if(child.Folder is not null && child.Id is not null)
            {
                var childPath = (child.ParentReference?.Path ?? parentPath) + "/" + child.Name;
                await InsertChildrenRecursiveAsync(driveId, child.Id, childPath, vm, token);
            }
        }
    }

    public async Task DownloadFilesAsync(MainWindowViewModel vm, UserSettings userSettings, CancellationToken token)
    {
        var metrics = new MetricsCollector();
        var downloadRoot = "/home/jason/Documents/OneDriveDownloads";

        Drive? drive = await _client.Me.Drive.GetAsync(cancellationToken: token);
        var driveId = drive!.Id!;

        // Ensure DB is bootstrapped BEFORE we count files
        LocalDriveItem? rootItem = await _store.GetRootAsync(driveId, token);
        if(rootItem is null)
        {
            vm.ReportProgress("ℹ️ Bootstrapping DB from Graph...");
            await BootstrapDriveAsync(driveId, vm, token);
            rootItem = await _store.GetRootAsync(driveId, token);
            vm.ReportProgress("✅ DB fully populated from Graph");
        }

        // Now count
        var totalFiles = await _store.CountTotalFilesAsync(token);
        metrics.Start(totalFiles);
        var reporter = new ProgressReporter(vm, metrics, fileInterval: 5, msInterval: 500);

        var downloadedIds = new ConcurrentBag<string>();
        using var semaphore = new SemaphoreSlim(userSettings.MaxParallelDownloads);

        await TraverseAndDownloadAsync(rootItem!, driveId, downloadRoot, vm, downloadedIds, semaphore, metrics, reporter, token);

        // Batch update
        var batch = new List<string>();
        foreach(var id in downloadedIds)
        {
            batch.Add(id);
            if(batch.Count >= userSettings.DownloadBatchSize)
            {
                await _store.MarkItemsAsDownloadedAsync(batch, token);
                batch.Clear();
            }
        }

        if(batch.Count > 0)
            await _store.MarkItemsAsDownloadedAsync(batch, token);

        reporter.Flush();
        vm.ReportProgress(metrics.GetGlobalSummary());
        foreach((var folder, var files, var mb) in metrics.GetFolderSummaries())
            vm.ReportProgress($"📂 {folder}: {files} files, {mb:F2} MB");
    }

    private async Task TraverseAndDownloadAsync(
        LocalDriveItem item,
        string driveId,
        string parentLocalPath,
        MainWindowViewModel vm,
        ConcurrentBag<string> downloadedIds,
        SemaphoreSlim semaphore,
        MetricsCollector metrics,
        ProgressReporter reporter,
        CancellationToken token)
    {
        var localPath = Path.Combine(parentLocalPath, item.Name ?? string.Empty);

        if(item.IsFolder)
        {
            // Ensure folder exists locally
            _ = Directory.CreateDirectory(localPath);
            vm.ReportProgress($"📂 Created folder {localPath}");

            // Get children from DB
            IReadOnlyList<LocalDriveItem> children = await _store.GetChildrenAsync(item.Id, token);
            foreach(LocalDriveItem child in children)
            {
                await TraverseAndDownloadAsync(child, driveId, localPath, vm, downloadedIds, semaphore, metrics, reporter, token);
            }
        }
        else
        {
            await semaphore.WaitAsync(token);
            _ = Task.Run(async () =>
            {
                try
                {
                    // Download file content from Graph
                    Stream? stream;
                    try
                    {
                        stream = await _client.Drives[driveId].Items[item.Id].Content.GetAsync(null, token);
                    }
                    catch(ODataError ex) when(ex.Error?.Code == "itemNotFound")
                    {
                        // Retry after short delay
                        await Task.Delay(1000, token);
                        stream = await _client.Drives[driveId].Items[item.Id].Content.GetAsync(null, token);
                    }

                    if(stream == null)
                    {
                        vm.ReportProgress($"⚠️ Failed to download {item.Name}: Stream is null");
                        return;
                    }

                    // Ensure parent directory exists
                    _ = Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

                    // Write file to disk
                    using FileStream fileStream = File.Create(localPath);
                    await stream.CopyToAsync(fileStream, token);

                    // Mark as downloaded
                    vm.ReportProgress($"⬇️ Downloaded {localPath}");
                    downloadedIds.Add(item.Id);

                    // Record metrics + throttled progress update
                    metrics.RecordFile(Path.GetDirectoryName(localPath)!, fileStream.Length);
                    reporter.OnFileCompleted();
                }
                catch(Exception ex)
                {
                    vm.ReportProgress($"⚠️ Failed to download {item.Name}: {ex.Message}");
                    metrics.RecordError();
                }
                finally
                {
                    _ = semaphore.Release();
                }
            }, token);
        }
    }

    /// <summary>
    /// Get a DriveItem by its path relative to root.
    /// Example path: "/Documents/MyFile.txt"
    /// </summary>
    public async Task<Result<DriveItem?, Exception>> GetItemByPathAsync(string path)
    {
        logger.LogDebug("Getting item by path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await loginService.CreateGraphServiceClientAsync();
            if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            try
            {
                Drive? drive = await client.Me.Drive.GetAsync();
                if(drive?.Id is null)
                {
                    logger.LogWarning("Unable to determine user's Drive id");
                    return (DriveItem?)null;
                }

                DriveItem? driveItem = await client.Drives[drive.Id].Root.ItemWithPath(normalized).GetAsync();
                return driveItem;
            }
            catch(Microsoft.Kiota.Abstractions.ApiException ex) when(ex.ResponseStatusCode == 404)
            {
                logger.LogDebug("Item not found at path: {Path}", path);
                return (DriveItem?)null;
            }
        });
    }

    /// <summary>
    /// Download the content of a file at a given path
    /// </summary>
    public async Task<Result<System.IO.Stream, Exception>> DownloadFileAsync(string path)
    {
        logger.LogInformation("Downloading file at path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await loginService.CreateGraphServiceClientAsync();
            if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            Drive? drive = await client.Me.Drive.GetAsync();
            if(drive?.Id is null)
            {
                logger.LogWarning("Unable to determine user's Drive id");
                return Stream.Null;
            }

            System.IO.Stream? stream = await client.Drives[drive.Id].Root.ItemWithPath(normalized).Content.GetAsync();
            return stream!;
        });
    }

    /// <summary>
    /// Uploads a file to a specific path (overwrites if exists)
    /// </summary>
    public async Task<Result<DriveItem, Exception>> UploadFileAsync(string path, System.IO.Stream content)
    {
        logger.LogInformation("Uploading file to path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await loginService.CreateGraphServiceClientAsync();
            if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            Drive? drive = await client.Me.Drive.GetAsync();
            if(drive?.Id is null)
            {
                logger.LogWarning("Unable to determine user's Drive id");
                throw new InvalidOperationException("Drive id not available");
            }

            DriveItem? uploaded = await client.Drives[drive.Id].Root.ItemWithPath(normalized).Content.PutAsync(content);
            return uploaded!;
        });
    }

    /// <summary>
    /// Creates a folder at a given path under root
    /// </summary>
    public async Task<Result<DriveItem, Exception>> CreateFolderAsync(string folderPath, string folderName)
    {
        // Prepare folder payload
        var folderToCreate = new DriveItem
        {
            Name = folderName,
            Folder = new Folder(),
            AdditionalData = new Dictionary<string, object>
            {
                {"@microsoft.graph.conflictBehavior", "rename"}
            }
        };

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await loginService.CreateGraphServiceClientAsync();
            if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;

            // If folderPath is null/empty or "/", create under root
            var normalizedParent = folderPath?.TrimStart('/') ?? string.Empty;

            Drive? drive = await client.Me.Drive.GetAsync();
            if(drive?.Id is null)
            {
                logger.LogWarning("Unable to determine user's Drive id");
                throw new InvalidOperationException("Drive id not available");
            }

            // Build RequestInformation for creating the child folder. Use explicit RequestInformation
            // so tests can intercept the outgoing request regardless of generated SDK shape.
            var request = new RequestInformation
            {
                HttpMethod = Method.POST,
                UrlTemplate = string.IsNullOrEmpty(normalizedParent)
                    ? "https://graph.microsoft.com/v1.0/drives/{driveId}/root/children"
                    : "https://graph.microsoft.com/v1.0/drives/{driveId}/root:/{parent}:/children",
                PathParameters = string.IsNullOrEmpty(normalizedParent)
                    ? new Dictionary<string, object> { { "driveId", drive.Id } }
                    : new Dictionary<string, object> { { "driveId", drive.Id }, { "parent", normalizedParent } }
            };

            request.SetContentFromParsable(client.RequestAdapter, "application/json", folderToCreate);

            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "4XX", ODataError.CreateFromDiscriminatorValue },
                { "5XX", ODataError.CreateFromDiscriminatorValue }
            };

            DriveItem? created = await client.RequestAdapter.SendAsync<DriveItem>(request, DriveItem.CreateFromDiscriminatorValue, errorMapping);
            return created!;
        });
    }
}
