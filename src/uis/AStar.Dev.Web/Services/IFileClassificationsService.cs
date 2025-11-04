using AStar.Dev.Web.Models;

namespace AStar.Dev.Web.Services;

/// <summary>
/// Service interface for fetching file classifications.
/// </summary>
public interface IFileClassificationsService
{
    /// <summary>
    /// Retrieves a list of distinct file classifications.
    /// </summary>
    /// <returns>A collection of file classification names.</returns>
    Task<IList<FileClassification>> GetFileClassificationsAsync();
}
