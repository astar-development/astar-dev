using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
/// Service responsible for retrieving a list of files from a directory.
/// This interface exists to allow easier unit testing of classes that depend on file list retrieval.
/// </summary>
public interface IFileListService
{
    /// <summary>
    /// Retrieves a list of <see cref="FileDetail"/> instances for files under the provided path.
    /// </summary>
    /// <param name="path">Root directory to enumerate.</param>
    /// <param name="stoppingToken">Cancellation token to observe.</param>
    /// <returns>A Result containing the list of file details or an ErrorResponse.</returns>
    Task<Result<List<FileDetail>, ErrorResponse>> Get(string path, CancellationToken stoppingToken);
}
