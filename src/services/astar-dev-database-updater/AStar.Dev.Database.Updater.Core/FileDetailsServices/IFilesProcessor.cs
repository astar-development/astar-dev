using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
/// Processes a collection of <see cref="FileDetail"/> objects for keywords and classifications.
/// Exposed as an interface to allow mocking or faking during unit tests.
/// </summary>
public interface IFilesProcessor
{
    /// <summary>
    /// Processes the provided files and persists any findings.
    /// </summary>
    /// <param name="filesToProcess">Files to process.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A Result containing true on success or an ErrorResponse on failure.</returns>
    Task<Result<bool, ErrorResponse>> ProcessAsync(IReadOnlyCollection<FileDetail> filesToProcess, CancellationToken cancellationToken);
}
