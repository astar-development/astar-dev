using Microsoft.Graph;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Extensions.Logging;
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
            : base(message, inner)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Lists items in the user's OneDrive root folder.
    /// </summary>
    public async Task<List<DriveItem>> GetRootItemsAsync()
    {
        _logger.LogInformation("Listing root items from OneDrive");
        GraphServiceClient client = await _loginService.SignInAsync();

        try
        {
            // Request the Drive resource and expand the root's children so
            // the Root.Children navigation property is populated in the returned Drive.
            Drive? drive = await client.Me.Drive.GetAsync(rc => rc.QueryParameters.Expand = new[] { "root($expand=children)" });

            List<DriveItem>? rootChildren = drive?.Root?.Children;
            List<DriveItem> results = rootChildren?.ToList() ?? new List<DriveItem>();
            _logger.LogInformation("Found {Count} root items", results.Count);
            return results;
        }
        catch (Microsoft.Kiota.Abstractions.ApiException apiEx)
        {
            _logger.LogError(apiEx, "Graph API error listing root items: {Status}", apiEx.ResponseStatusCode);
            throw new OneDriveServiceException("Failed to list root items", (int?)apiEx.ResponseStatusCode, apiEx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error listing root items");
            throw new OneDriveServiceException("Unexpected error listing root items", null, ex);
        }
    }

    /// <summary>
    /// Get a DriveItem by its path relative to root.
    /// Example path: "/Documents/MyFile.txt"
    /// </summary>
    public async Task<DriveItem?> GetItemByPathAsync(string path)
    {
        _logger.LogDebug("Getting item by path: {Path}", path);
        GraphServiceClient client = await _loginService.SignInAsync();
        // Normalize path (remove leading slash if provided)
        var normalized = path?.TrimStart('/') ?? string.Empty;

        try
        {
            Drive? drive = await client.Me.Drive.GetAsync();
            if (drive?.Id is null)
            {
                _logger.LogWarning("Unable to determine user's Drive id");
                return null;
            }
            DriveItem? driveItem = await client.Drives[drive.Id].Root.ItemWithPath(normalized).GetAsync();
            return driveItem;
        }
        catch (Microsoft.Kiota.Abstractions.ApiException ex) when (ex.ResponseStatusCode == 404)
        {
            _logger.LogDebug("Item not found at path: {Path}", path);
            return null;
        }
    }

    /// <summary>
    /// Download the content of a file at a given path
    /// </summary>
    public async Task<System.IO.Stream> DownloadFileAsync(string path)
    {
        _logger.LogInformation("Downloading file at path: {Path}", path);
        GraphServiceClient client = await _loginService.SignInAsync();
        var normalized = path?.TrimStart('/') ?? string.Empty;

        // Request the content stream from the item at the provided path
        Drive? drive = await client.Me.Drive.GetAsync();
        if (drive?.Id is null)
        {
            _logger.LogWarning("Unable to determine user's Drive id");
            return System.IO.Stream.Null;
        }
        System.IO.Stream? stream = await client.Drives[drive.Id].Root.ItemWithPath(normalized).Content.GetAsync();
        return stream!;
    }

    /// <summary>
    /// Uploads a file to a specific path (overwrites if exists)
    /// </summary>
    public async Task<DriveItem> UploadFileAsync(string path, System.IO.Stream content)
    {
        _logger.LogInformation("Uploading file to path: {Path}", path);
        var normalized = path?.TrimStart('/') ?? string.Empty;

        // Upload (PUT) the provided stream to the given path. This will create or replace.
        GraphServiceClient client = await _loginService.SignInAsync();
        Drive? drive = await client.Me.Drive.GetAsync();
        if (drive?.Id is null)
        {
            _logger.LogWarning("Unable to determine user's Drive id");
            throw new InvalidOperationException("Drive id not available");
        }
        DriveItem? uploaded = await client.Drives[drive.Id].Root.ItemWithPath(normalized).Content.PutAsync(content);
        return uploaded!;
    }

    /// <summary>
    /// Creates a folder at a given path under root
    /// </summary>
    public async Task<DriveItem> CreateFolderAsync(string folderPath, string folderName)
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

        GraphServiceClient client = await _loginService.SignInAsync();

        // If folderPath is null/empty or "/", create under root
        var normalizedParent = folderPath?.TrimStart('/') ?? string.Empty;

        Drive? drive = await client.Me.Drive.GetAsync();
        if (drive?.Id is null)
        {
            _logger.LogWarning("Unable to determine user's Drive id");
            throw new InvalidOperationException("Drive id not available");
        }

        if (string.IsNullOrEmpty(normalizedParent))
        {
            var request = new RequestInformation
            {
                HttpMethod = Method.POST,
                UrlTemplate = "https://graph.microsoft.com/v1.0/drives/{driveId}/root/children",
                PathParameters = new Dictionary<string, object> { { "driveId", drive.Id } }
            };

            request.SetContentFromParsable(client.RequestAdapter, "application/json", folderToCreate);

            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "4XX", ODataError.CreateFromDiscriminatorValue },
                { "5XX", ODataError.CreateFromDiscriminatorValue }
            };

            DriveItem? created = await client.RequestAdapter.SendAsync<DriveItem>(request, DriveItem.CreateFromDiscriminatorValue, errorMapping);
            return created!;
        }
        else
        {
            var request = new RequestInformation
            {
                HttpMethod = Method.POST,
                UrlTemplate = "https://graph.microsoft.com/v1.0/drives/{driveId}/root:/{parent}:/children",
                PathParameters = new Dictionary<string, object> { { "driveId", drive.Id }, { "parent", normalizedParent } }
            };

            request.SetContentFromParsable(client.RequestAdapter, "application/json", folderToCreate);

            var errorMapping = new Dictionary<string, ParsableFactory<IParsable>>
            {
                { "4XX", ODataError.CreateFromDiscriminatorValue },
                { "5XX", ODataError.CreateFromDiscriminatorValue }
            };

            DriveItem? created = await client.RequestAdapter.SendAsync<DriveItem>(request, DriveItem.CreateFromDiscriminatorValue, errorMapping);
            return created!;
        }
    }
}
