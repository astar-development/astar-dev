using System.Net.Http.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Files.Api.Client.SDK.FilesApi;

/// <summary>
///     The <see href="FilesApiClient"></see> class
/// </summary>
/// <param name="httpClient"></param>
/// <param name="logger"></param>
public class FilesApiClient(HttpClient httpClient, /*ITokenAcquisition tokenAcquisitionService,*/ ILogger<FilesApiClient> logger) : IFilesApiClient
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    /// <inheritdoc />
    public async Task<HealthStatusResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Checking the {ApiName} Health Status.", Constants.ApiName);

        try
        {
            HttpResponseMessage response = await httpClient.GetAsync("/health/ready", cancellationToken);

            return response.IsSuccessStatusCode
                ? await ReturnLoggedSuccess(response)
                : ReturnLoggedFailure(response);
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return new() { Status = $"Could not get a response from the {Constants.ApiName}." };
        }
    }

    /// <summary>
    ///     The GetFilesCountAsync method will get the count of the files that match the search parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the files count
    /// </param>
    /// <returns>The count of the matching files or -1 if an error occurred</returns>
    public async Task<int> GetFilesCountAsync(SearchParameters searchParameters)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"files/count?{searchParameters.ToQueryString()}&version=1");

        logger.LogInformation("Getting the count of matching files.");

        return response.IsSuccessStatusCode
            ? int.Parse(await response.Content.ReadAsStringAsync())
            : -1;
    }

    /// <summary>
    ///     The GetFilesAsync method will, as its name suggests, get the files that match the search parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the files search
    /// </param>
    /// <returns>An enumerable list of <see href="FileDetail"></see> instances</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IEnumerable<FileDetail>> GetFilesAsync(SearchParameters searchParameters)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"files/list?{searchParameters.ToQueryString()}&version=1");

        logger.LogInformation("Getting the list of files matching the criteria.");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? content.FromJson<IEnumerable<FileDetail>>(Utilities.Constants.WebDeserialisationSettings)
            : throw new InvalidOperationException(content);
    }

    /// <summary>
    ///     The GetDuplicateFilesAsync method will, as its name suggests, get the duplicate files that match the search
    ///     parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the duplicate files search
    /// </param>
    /// {
    /// <returns>An enumerable list of <see href="DuplicateGroup"></see> instances</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<IReadOnlyCollection<DuplicatesList>> GetDuplicateFilesAsync(SearchParameters searchParameters)
    {
        var queryString = searchParameters.ToQueryString();
        logger.LogInformation("Getting the list of duplicate files matching the criteria: {SearchParameters}", queryString);

        var token = string.Empty; // await tokenAcquisitionService.GetAccessTokenForUserAsync(["api://54861ab2-fdb0-4e18-a073-c90e7bf9f0c5/ToDoList.Write",]);

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

        HttpResponseMessage response = await httpClient.GetAsync($"duplicates?{queryString}&version=1");
        logger.LogDebug("Response.StatusCode: {Response_StatusCode}", response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<DuplicatesList>>())!;
    }

    /// <summary>
    ///     The GetFileClassificationsAsync method will get all file classifications
    /// </summary>
    /// <returns>A collection of file classifications</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual async Task<IReadOnlyCollection<FileClassification>> GetFileClassificationsAsync()
    {
        logger.LogInformation("Getting the list of file classifications.");

        HttpResponseMessage response = await httpClient.GetAsync("api/file-classifications?version=1");
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? content.FromJson<IReadOnlyCollection<FileClassification>>(Utilities.Constants.WebDeserialisationSettings)
            : throw new InvalidOperationException(content);
    }

    /// <summary>
    ///     The GetDuplicateFilesCountAsync method will get the count of the duplicate files that match the search parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the duplicate files count
    /// </param>
    /// <returns>The count of the matching duplicate files or -1 if an error occurred</returns>
    public async Task<GetDuplicatesCountQueryResponse> GetDuplicateFilesCountAsync(SearchParameters searchParameters)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"/duplicates/count?{searchParameters.ToQueryString()}&version=1");

        logger.LogInformation("Getting the count of matching duplicate files.");

        GetDuplicatesCountQueryResponse? getDuplicatesCountQueryResponse = await response.Content.ReadFromJsonAsync<GetDuplicatesCountQueryResponse>();

        return (response.IsSuccessStatusCode
            ? getDuplicatesCountQueryResponse
            : new(-1))!;
    }

    /// <summary>
    ///     The GetFileAccessDetail method will, as its name suggests, get the file access details for the specified file
    /// </summary>
    /// <param name="fileId">The ID of the file to retrieve the File Access Details from the database</param>
    /// <returns>An instance of <see href="FileAccessDetail"></see> for the specified File ID</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<FileAccessDetail> GetFileAccessDetail(int fileId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"files/access-detail?request={fileId}&version=1");

        logger.LogInformation("Getting the access detail for the file with ClassificationId: {FileId}.", fileId);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? content.FromJson<FileAccessDetail>(Utilities.Constants.WebDeserialisationSettings)
            : throw new InvalidOperationException(content);
    }

    /// <summary>
    ///     The GetFileDetail method will, as its name suggests, get the file details of the specified file
    /// </summary>
    /// <param name="fileId">The ID of the file detail to retrieve from the database</param>
    /// <returns>
    ///     An awaitable task containing an instance of <see href="FileDetail"></see> containing the, you guessed it, File
    ///     details...
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<FileDetail> GetFileDetail(int fileId)
    {
        HttpResponseMessage response = await httpClient.GetAsync($"files/detail?request={fileId}&version=1");

        logger.LogInformation("Getting the file detail for the file with ClassificationId: {FileId}.", fileId);
        var content = await response.Content.ReadAsStringAsync();

        return response.IsSuccessStatusCode
            ? content.FromJson<FileDetail>(Utilities.Constants.WebDeserialisationSettings)
            : throw new InvalidOperationException(content);
    }

    /// <summary>
    ///     The MarkForSoftDeletionAsync method will, as its name suggests, mark the specified file as soft deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to mark as soft deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    public async Task<string> MarkForSoftDeletionAsync(int fileId)
    {
        try
        {
            logger.LogInformation("Marking the fileId: {FileId} for soft deletion.", fileId);

            var content = new StringContent(JsonSerializer.Serialize(new { fileId }), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PutAsync("soft-delete/mark?version=1", content);

            logger.LogDebug("Marking the fileId: {FileId} for soft deletion returned: {ResponseStatus}.", fileId, response.StatusCode);

            return response.IsSuccessStatusCode
                ? "Marked for soft deletion"
                : await response.Content.ReadAsStringAsync();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return ex.Message;
        }
    }

    /// <summary>
    ///     The UndoMarkForSoftDeletionAsync method will, as its name suggests, unmark the specified file as soft deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to unmark as soft deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    public async Task<string> UndoMarkForSoftDeletionAsync(int fileId)
    {
        try
        {
            logger.LogInformation("Unmarking the fileId: {FileId} for soft deletion.", fileId);

            var content = new StringContent(JsonSerializer.Serialize(new { fileId }), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PutAsync("soft-delete/unmark?version=1", content);

            logger.LogDebug("Unmarking the fileId: {FileId} for soft deletion returned: {ResponseStatus}.", fileId, response.StatusCode);

            return response.IsSuccessStatusCode
                ? "Mark for soft deletion has been undone"
                : await response.Content.ReadAsStringAsync();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return ex.Message;
        }
    }

    /// <summary>
    ///     The MarkForHardDeletionAsync method will, as its name suggests, mark the specified file as hard deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to mark as hard deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    public async Task<string> MarkForHardDeletionAsync(int fileId)
    {
        try
        {
            logger.LogInformation("Marking the fileId: {FileId} for hard deletion.", fileId);

            var content = new StringContent(JsonSerializer.Serialize(new { fileId }), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PutAsync("hard-delete/mark?version=1", content);

            logger.LogDebug("Marking the fileId: {FileId} for hard deletion returned: {ResponseStatus}.", fileId, response.StatusCode);

            return response.IsSuccessStatusCode
                ? "Marked for hard deletion."
                : await response.Content.ReadAsStringAsync();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return ex.Message;
        }
    }

    /// <summary>
    ///     The UndoMarkForHardDeletionAsync method will, as its name suggests, unmark the specified file as hard deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to unmark as hard deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    public async Task<string> UndoMarkForHardDeletionAsync(int fileId)
    {
        try
        {
            logger.LogInformation("Unmarking the fileId: {FileId} for hard deletion.", fileId);

            var content = new StringContent(JsonSerializer.Serialize(new { fileId }), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PutAsync("hard-delete/unmark?version=1", content);

            logger.LogDebug("Unmarking the fileId: {FileId} for hard deletion returned: {ResponseStatus}.", fileId, response.StatusCode);

            return response.IsSuccessStatusCode
                ? "Mark for hard deletion has been undone"
                : await response.Content.ReadAsStringAsync();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return ex.Message;
        }
    }

    /// <summary>
    ///     The MarkForMovingAsync method will, as its name suggests, mark the specified file as requiring moving
    /// </summary>
    /// <param name="fileId">The ID of the file to mark as move required</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    public async Task<string> MarkForMovingAsync(int fileId)
    {
        try
        {
            logger.LogInformation("Marking the fileId: {FileId} for moving.", fileId);

            var content = new StringContent(JsonSerializer.Serialize(new { fileId }), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PutAsync("move/mark?version=1", content);

            logger.LogDebug("Marking the fileId: {FileId} for moving returned: {ResponseStatus}.", fileId, response.StatusCode);

            return response.IsSuccessStatusCode
                ? "Mark for moving was successful"
                : await response.Content.ReadAsStringAsync();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return ex.Message;
        }
    }

    /// <summary>
    ///     The UndoMarkForMovingAsync method will, as its name suggests, unmark the specified file as requiring moving
    /// </summary>
    /// <param name="fileId">The ID of the file to unmark as move required</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    public async Task<string> UndoMarkForMovingAsync(int fileId)
    {
        try
        {
            logger.LogInformation("Unmarking the fileId: {FileId} for moving.", fileId);

            var content = new StringContent(JsonSerializer.Serialize(new { fileId }), Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await httpClient.PutAsync("move/unmark?version=1", content);

            logger.LogDebug("UnMarking the fileId: {FileId} for moving returned: {ResponseStatus}.", fileId, response.StatusCode);

            return response.IsSuccessStatusCode
                ? "Undo mark for moving was successful"
                : await response.Content.ReadAsStringAsync();
        }
        catch(HttpRequestException ex)
        {
            logger.LogError(500, ex, "Error: {ErrorMessage}", ex.Message);

            return ex.Message;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<string> UpdateFileDirectoryAsync(DirectoryChangeRequest request) => throw new NotImplementedException();

    /// <summary>
    /// </summary>
    /// <param name="fileId"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<string> UpdateFileLastViewedAsync(int fileId) => throw new NotImplementedException();

    /// <summary>
    /// </summary>
    /// <param name="fileDetail"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<string> PushFileDetailAsync(FileDetail fileDetail) => throw new NotImplementedException();

    /// <summary>
    /// </summary>
    /// <param name="fileDetails"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<string> PushFileDetailsAsync(IEnumerable<FileDetail> fileDetails) => throw new NotImplementedException();

    /// <summary>
    ///     The UpdateFileAsync method will, as the name suggests, update the file - currently, the directory is the only thing
    ///     to change
    /// </summary>
    /// <param name="directoryChangeRequest">
    ///     An instance of the <see href="DirectoryChangeRequest"></see> class used to control the file update
    /// </param>
    /// <returns>An awaitable task of <see cref="string" /></returns>
    public async Task<string> UpdateFileAsync(DirectoryChangeRequest directoryChangeRequest)
    {
        var httpContent = new StringContent(directoryChangeRequest.ToString(), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PutAsync("files/update-directory?version=1", httpContent);

        _ = response.EnsureSuccessStatusCode();

        logger.LogDebug("Update File {DirectoryChangeRequest} response was: {Status}",
            directoryChangeRequest.ToString(), response.StatusCode);

        return response.IsSuccessStatusCode
            ? "The file details were updated successfully"
            : await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    ///     The GetDirectoriesAsync method returns all directories under the supplied root Directory
    /// </summary>
    /// <param name="rootDirectory">The root (starting) directory</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken" />...I know...</param>
    /// <returns>An awaitable task of enumerable of <see cref="string" /></returns>
    public async Task<IEnumerable<string>> GetDirectoriesAsync(string rootDirectory, CancellationToken cancellationToken)
    {
        rootDirectory = UrlEncoder.Default.Encode(rootDirectory);

        logger.LogInformation("Starting GetDirectoriesAsync for rootDirectory: {RootDirectory} with baseUrl: {BaseUrl}", $"/directories/{rootDirectory}?version=1",
                              httpClient.BaseAddress);

        var token = string.Empty;
        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", token);

        HttpResponseMessage response = await httpClient.GetAsync($"/directories/{rootDirectory}?version=1", cancellationToken);

        _ = response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<string[]>(cancellationToken))!;
    }

    private async Task<HealthStatusResponse> ReturnLoggedSuccess(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health check completed successfully.", Constants.ApiName);

        return (await JsonSerializer.DeserializeAsync<HealthStatusResponse>(await response.Content.ReadAsStreamAsync(),
            JsonSerializerOptions))!;
    }

    private HealthStatusResponse ReturnLoggedFailure(HttpResponseMessage response)
    {
        logger.LogInformation("The {ApiName} Health failed - {FailureReason}.", Constants.ApiName,
            response.ReasonPhrase);

        return new() { Status = $"Health Check failed - {response.ReasonPhrase}." };
    }
}
