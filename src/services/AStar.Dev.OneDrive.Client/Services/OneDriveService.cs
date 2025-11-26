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

public sealed class OneDriveService
{
    private readonly ILoginService _loginService;
    private readonly UserSettings _userSettings;
    private readonly ILogger<OneDriveService> _logger;
    private DeltaStore _store = null!;
    private GraphServiceClient _client = null!;

    public OneDriveService(ILoginService loginService, UserSettings userSettings, ILogger<OneDriveService> logger)
    {
        _loginService = loginService;
        _userSettings = userSettings;
        _logger = logger;
    }

    // Custom exception type for higher-level error surface
    public sealed class OneDriveServiceException : Exception
    {
        public int? StatusCode { get; }

        public OneDriveServiceException(string message, int? statusCode = null, Exception? inner = null)
            : base(message, inner) => StatusCode = statusCode;
    }

    /// <summary>
    /// Lists items in the user's OneDrive root folder.
    /// Returns a Result containing the list or the captured exception.
    /// </summary>
    public async Task GetRootItemsAsync(MainWindowViewModel vm, CancellationToken token)
    {
        _logger.LogInformation("Listing root items from OneDrive");

        Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
        if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
        {
            throw loginErr.Reason;
        }

        _client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
        // Resolve the user's home directory in a cross-platform way
        var appDataPath = AppPathHelper.GetAppDataPath("onedrive-sync");
        _ = Directory.CreateDirectory(appDataPath);

        var dbPath = Path.Combine(appDataPath, "onedrive_sync.db");
        _store = new DeltaStore(_logger, dbPath);

        var syncManager = new SyncManager(_client, _store, vm, token);

        await syncManager.RunSyncAsync();
        if(vm.DownloadFilesAfterSync)
        {
            await DownloadFilesAsync(vm, _userSettings, token);
        }
    }
    public async Task BootstrapDriveAsync(string driveId, CancellationToken token)
    {
        // Fetch root item from Graph
        DriveItem? rootItem = await _client.Drives[driveId].Items["root"]
        .GetAsync(cancellationToken: token);

        if(rootItem == null)
            return;

        await _store.InsertRootAsync(driveId, rootItem, token);

        // Recursively insert children
        await InsertChildrenRecursiveAsync(driveId, rootItem.Id!, $"/drives/{driveId}/root:", token);
    }

    private async Task InsertChildrenRecursiveAsync(string driveId, string itemId, string parentPath, CancellationToken token)
    {
        // Get children of this folder
        DriveItemCollectionResponse? childrenResponse =
        await _client.Drives[driveId].Items[itemId].Children
            .GetAsync(cancellationToken: token);

        if(childrenResponse?.Value == null)
            return;

        // Insert into DB
        await _store.InsertChildrenAsync(parentPath, childrenResponse.Value, token);

        // Recurse into subfolders
        foreach(DriveItem child in childrenResponse.Value)
        {
            if(child.Folder != null && child.Id != null)
            {
                var childPath = (child.ParentReference?.Path ?? parentPath) + "/" + child.Name;
                await InsertChildrenRecursiveAsync(driveId, child.Id, childPath, token);
            }
        }
    }

    public async Task DownloadFilesAsync(MainWindowViewModel vm, UserSettings userSettings, CancellationToken token)
    {
        var downloadRoot = "/home/jason/Documents/OneDriveDownloads";

        Drive? drive = await _client.Me.Drive.GetAsync(cancellationToken: token);
        var driveId = drive!.Id!;

        // Try to get root item from DB
        LocalDriveItem? rootItem = await _store.GetRootAsync(driveId, token);
        if(rootItem == null)
        {
            vm.ReportProgress("ℹ️ Bootstrapping DB from Graph...");
            await BootstrapDriveAsync(driveId, token);
            rootItem = await _store.GetRootAsync(driveId, token);
            vm.ReportProgress("✅ DB fully populated from Graph");
        }

        var downloadedIds = new ConcurrentBag<string>();
        using var semaphore = new SemaphoreSlim(userSettings.MaxParallelDownloads);

        // Kick off traversal from root
        await TraverseAndDownloadAsync(rootItem!, driveId, downloadRoot, vm, downloadedIds, semaphore, token);

        // Batch update in chunks
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
        {
            await _store.MarkItemsAsDownloadedAsync(batch, token);
        }
    }

    private async Task TraverseAndDownloadAsync(
        LocalDriveItem item,
        string driveId,
        string parentLocalPath,
        MainWindowViewModel vm,
        ConcurrentBag<string> downloadedIds,
        SemaphoreSlim semaphore,
        CancellationToken token)
    {
        var localPath = Path.Combine(parentLocalPath, item.Name ?? string.Empty);

        if(item.IsFolder)
        {
            _ = Directory.CreateDirectory(localPath);
            vm.ReportProgress($"📂 Created folder {localPath}");

            // Query DB for children by ParentPath
            IReadOnlyList<LocalDriveItem> children = await _store.GetChildrenAsync(item.Id, token);
            foreach(LocalDriveItem child in children)
            {
                await TraverseAndDownloadAsync(child, driveId, localPath, vm, downloadedIds, semaphore, token);
            }
        }
        else
        {
            await semaphore.WaitAsync(token);
            _ = Task.Run(async () =>
            {
                try
                {
                    using Stream? stream = await _client.Drives[driveId].Items[item.Id].Content.GetAsync(cancellationToken: token);
                    if(stream == null)
                    {
                        vm.ReportProgress($"⚠️ Failed to download {item.Name}: Stream is null");
                        return;
                    }

                    _ = Directory.CreateDirectory(Path.GetDirectoryName(localPath)!);

                    using FileStream fileStream = File.Create(localPath);
                    await stream.CopyToAsync(fileStream, token);

                    vm.ReportProgress($"⬇️ Downloaded {localPath}");
                    downloadedIds.Add(item.Id);
                }
                catch(Exception ex)
                {
                    vm.ReportProgress($"⚠️ Failed to download {item.Name}: {ex.Message}");
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
        _logger.LogDebug("Getting item by path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
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
                    _logger.LogWarning("Unable to determine user's Drive id");
                    return (DriveItem?)null;
                }

                DriveItem? driveItem = await client.Drives[drive.Id].Root.ItemWithPath(normalized).GetAsync();
                return driveItem;
            }
            catch(Microsoft.Kiota.Abstractions.ApiException ex) when(ex.ResponseStatusCode == 404)
            {
                _logger.LogDebug("Item not found at path: {Path}", path);
                return (DriveItem?)null;
            }
        });
    }

    /// <summary>
    /// Download the content of a file at a given path
    /// </summary>
    public async Task<Result<System.IO.Stream, Exception>> DownloadFileAsync(string path)
    {
        _logger.LogInformation("Downloading file at path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
            if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            Drive? drive = await client.Me.Drive.GetAsync();
            if(drive?.Id is null)
            {
                _logger.LogWarning("Unable to determine user's Drive id");
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
        _logger.LogInformation("Uploading file to path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
            if(loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            Drive? drive = await client.Me.Drive.GetAsync();
            if(drive?.Id is null)
            {
                _logger.LogWarning("Unable to determine user's Drive id");
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
            Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
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
                _logger.LogWarning("Unable to determine user's Drive id");
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
