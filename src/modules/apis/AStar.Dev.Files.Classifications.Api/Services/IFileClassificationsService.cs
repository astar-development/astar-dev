namespace AStar.Dev.Files.Classifications.Api.Services;

/// <summary>
/// Service interface for fetching file classifications.
/// </summary>
public interface IFileClassificationsService2
{
    /// <summary>
    /// Retrieves a list of distinct file classifications.
    /// </summary>
    /// <returns>A collection of file classification names.</returns>
    Task<IList<FileClassification>> GetFileClassificationsAsync();
}
