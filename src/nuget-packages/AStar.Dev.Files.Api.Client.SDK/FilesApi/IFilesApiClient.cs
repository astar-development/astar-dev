using AStar.Dev.Api.HealthChecks;
using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.SDK.FilesApi;

/// <summary>
///     Interface for Files API Client
/// </summary>
public interface IFilesApiClient : IApiClient
{
    /// <summary>
    ///     The GetFilesCountAsync method will get the count of the files that match the search parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the files count
    /// </param>
    /// <returns>The count of the matching files or -1 if an error occurred</returns>
    Task<int> GetFilesCountAsync(SearchParameters searchParameters);

    /// <summary>
    ///     The GetFilesAsync method will, as its name suggests, get the files that match the search parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the files search
    /// </param>
    /// <returns>An enumerable list of <see href="FileDetail"></see> instances</returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<IEnumerable<FileDetail>> GetFilesAsync(SearchParameters searchParameters);

    /// <summary>
    ///     The GetDuplicateFilesAsync method will, as its name suggests, get the duplicate files that match the search
    ///     parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the duplicate files search
    /// </param>
    /// <returns>An enumerable list of <see href="DuplicateGroup"></see> instances</returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<IReadOnlyCollection<DuplicatesList>> GetDuplicateFilesAsync(SearchParameters searchParameters);

    /// <summary>
    ///     The GetFileClassificationsAsync method will get all file classifications
    /// </summary>
    /// <returns>A collection of file classifications</returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<IReadOnlyCollection<FileClassification>> GetFileClassificationsAsync();

    /// <summary>
    ///     The GetDuplicateFilesCountAsync method will get the count of the duplicate files that match the search parameters
    /// </summary>
    /// <param name="searchParameters">
    ///     An instance of the <see href="SearchParameters"></see> class defining the search
    ///     criteria for the duplicate files count
    /// </param>
    /// <returns>The count of the matching duplicate files or -1 if an error occurred</returns>
    Task<GetDuplicatesCountQueryResponse> GetDuplicateFilesCountAsync(SearchParameters searchParameters);

    /// <summary>
    ///     The GetFileAccessDetail method will, as its name suggests, get the file access details for the specified file
    /// </summary>
    /// <param name="fileId">The ID of the file to retrieve the File Access Details from the database</param>
    /// <returns>An instance of <see href="FileAccessDetail"></see> for the specified File ID</returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<FileAccessDetail> GetFileAccessDetail(int fileId);

    /// <summary>
    ///     The GetFileDetail method will, as its name suggests, get the file details of the specified file
    /// </summary>
    /// <param name="fileId">The ID of the file detail to retrieve from the database</param>
    /// <returns>
    ///     An awaitable task containing an instance of <see href="FileDetail"></see> containing the, you guessed it, File
    ///     details...
    /// </returns>
    /// <exception cref="InvalidOperationException"></exception>
    Task<FileDetail> GetFileDetail(int fileId);

    /// <summary>
    ///     The MarkForSoftDeletionAsync method will, as its name suggests, mark the specified file as soft deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to mark as soft deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> MarkForSoftDeletionAsync(int fileId);

    /// <summary>
    ///     The UndoMarkForSoftDeletionAsync method will, as its name suggests, unmark the specified file as soft deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to unmark as soft deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> UndoMarkForSoftDeletionAsync(int fileId);

    /// <summary>
    ///     The MarkForHardDeletionAsync method will, as its name suggests, mark the specified file as hard deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to mark as hard deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> MarkForHardDeletionAsync(int fileId);

    /// <summary>
    ///     The UndoMarkForHardDeletionAsync method will, as its name suggests, unmark the specified file as hard deleted
    /// </summary>
    /// <param name="fileId">The ID of the file to unmark as hard deleted</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> UndoMarkForHardDeletionAsync(int fileId);

    /// <summary>
    ///     The MarkForMovingAsync method will, as its name suggests, mark the specified file as requiring moving
    /// </summary>
    /// <param name="fileId">The ID of the file to mark as move required</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> MarkForMovingAsync(int fileId);

    /// <summary>
    ///     The UndoMarkForMovingAsync method will, as its name suggests, unmark the specified file as requiring moving
    /// </summary>
    /// <param name="fileId">The ID of the file to unmark as move required</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> UndoMarkForMovingAsync(int fileId);

    /// <summary>
    ///     The UpdateFileDirectoryAsync method will, as its name suggests, update the directory of the specified file
    /// </summary>
    /// <param name="request">
    ///     An instance of the <see href="DirectoryChangeRequest"></see> class containing the file ID and new
    ///     directory
    /// </param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> UpdateFileDirectoryAsync(DirectoryChangeRequest request);

    /// <summary>
    ///     The UpdateFileLastViewedAsync method will, as its name suggests, update the last viewed date of the specified file
    /// </summary>
    /// <param name="fileId">The ID of the file to update the last viewed date for</param>
    /// <returns>An awaitable task containing a string with the status of the update</returns>
    Task<string> UpdateFileLastViewedAsync(int fileId);

    /// <summary>
    ///     The PushFileDetailAsync method will, as its name suggests, push the file detail to the API
    /// </summary>
    /// <param name="fileDetail">The file detail to push to the API</param>
    /// <returns>An awaitable task containing a string with the status of the push</returns>
    Task<string> PushFileDetailAsync(FileDetail fileDetail);

    /// <summary>
    ///     The PushFileDetailsAsync method will, as its name suggests, push multiple file details to the API
    /// </summary>
    /// <param name="fileDetails">The file details to push to the API</param>
    /// <returns>An awaitable task containing a string with the status of the push</returns>
    Task<string> PushFileDetailsAsync(IEnumerable<FileDetail> fileDetails);
}
