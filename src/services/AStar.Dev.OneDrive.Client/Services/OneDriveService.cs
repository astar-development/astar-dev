using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Graph.Models.ODataErrors;
using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph.Models;

namespace AStar.Dev.OneDrive.Client.Services;

public class OneDriveService
{
    private readonly ILoginService _loginService;
    private readonly ILogger<OneDriveService> _logger;

    public OneDriveService(ILoginService loginService, ILogger<OneDriveService> logger)
    {
        _loginService = loginService;
        _logger = logger;
    }

    // Custom exception type for higher-level error surface
    public class OneDriveServiceException : Exception
    {
        public int? StatusCode { get; }

        public OneDriveServiceException(string message, int? statusCode = null, Exception? inner = null)
            : base(message, inner) => StatusCode = statusCode;
    }

    /// <summary>
    /// Lists items in the user's OneDrive root folder.
    /// Returns a Result containing the list or the captured exception.
    /// </summary>
    public async Task<Result<List<DriveItem>, Exception>> GetRootItemsAsync()
    {
        _logger.LogInformation("Listing root items from OneDrive");

        return await Try.RunAsync(async () =>
        {
            Result<GraphServiceClient, Exception> loginResult = await _loginService.SignInAsync();
            if (loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;

            // Get the drive (current user)
            Drive? drive = await RetryHelper.ExecuteWithBackoffAsync(() => client.Me.Drive.GetAsync());

            DriveItemCollectionResponse? rootChildren = await client.Drives[drive!.Id!].Items["root"].Children.GetAsync();

            List<DriveItem> results = rootChildren?.Value ?? new List<DriveItem>();
            _logger.LogInformation("Found {Count} root items", results.Count);
            return results;
        });
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
            if (loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            try
            {
                Drive? drive = await client.Me.Drive.GetAsync();
                if (drive?.Id is null)
                {
                    _logger.LogWarning("Unable to determine user's Drive id");
                    return (DriveItem?)null;
                }

                DriveItem? driveItem = await client.Drives[drive.Id].Root.ItemWithPath(normalized).GetAsync();
                return driveItem;
            }
            catch (Microsoft.Kiota.Abstractions.ApiException ex) when (ex.ResponseStatusCode == 404)
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
            if (loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            Drive? drive = await client.Me.Drive.GetAsync();
            if (drive?.Id is null)
            {
                _logger.LogWarning("Unable to determine user's Drive id");
                return System.IO.Stream.Null;
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
            if (loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;
            Drive? drive = await client.Me.Drive.GetAsync();
            if (drive?.Id is null)
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
            if (loginResult is Result<GraphServiceClient, Exception>.Error loginErr)
            {
                throw loginErr.Reason;
            }

            GraphServiceClient client = ((Result<GraphServiceClient, Exception>.Ok)loginResult).Value;

            // If folderPath is null/empty or "/", create under root
            var normalizedParent = folderPath?.TrimStart('/') ?? string.Empty;

            Drive? drive = await client.Me.Drive.GetAsync();
            if (drive?.Id is null)
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
