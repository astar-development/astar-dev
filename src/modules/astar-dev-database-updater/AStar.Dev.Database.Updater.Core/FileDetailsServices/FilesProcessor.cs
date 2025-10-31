using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
///     The FilesProcessor class is responsible for processing files to check for keywords
///     or specific classifications using provided services for classification and keyword detection.
/// </summary>
public class FilesProcessor(
    FilesContext filesContext,
    IKeywordProvider keywordProvider,
    FileDetailsProcessorService fileDetailsProcessorService,
    ILogger<FilesProcessor> logger)

    : IFilesProcessor
{
    /// <summary>
    ///     Processes a collection of files to process keywords, applying regex patterns to identify matches,
    ///     and persists relevant information into the database.
    /// </summary>
    /// <param name="filesToProcess">A read-only collection of FileDetail objects representing the files to be processed.</param>
    /// <param name="cancellationToken">An optional token to observe while waiting for the task to complete.</param>
    /// <returns>
    ///     A Result object that resolves to either:
    ///     - A boolean indicating success (true indicates successful processing).
    ///     - An ErrorResponse in case of a failure.
    /// </returns>
    public async Task<Result<bool, ErrorResponse>> ProcessAsync(IReadOnlyCollection<FileDetail> filesToProcess, CancellationToken cancellationToken)
    {
        var counter                        = 0;
        List<FileHandle> fileHandlesAlreadyInTheContext = await filesContext.Files.Select(f => f.FileHandle).ToListAsync(cancellationToken);

        List<FileClassification> classifications = await filesContext.FileClassifications
                                                .Include(fc => fc.FileNameParts)
                                                .ToListAsync(cancellationToken);

        IReadOnlyList<FileNamePartsWithClassifications> keywords = await keywordProvider.GetKeywordsAsync(cancellationToken);

        var              writeCount  = 0;
        List<FileDetail> fileDetails = [];

        foreach(FileDetail fileDetail in filesToProcess)
        {
            if(cancellationToken.IsCancellationRequested) break;

            try
            {
                (counter, writeCount) = fileDetailsProcessorService.ProcessFileDetailAsync(fileDetail, classifications, fileHandlesAlreadyInTheContext, counter, writeCount, keywords);

                fileDetails.Add(fileDetail);
                writeCount++;

                if(writeCount % 100 != 0) continue;

                _ = await SaveFileDetailsAsync(writeCount, fileDetails, cancellationToken);
                fileDetails.Clear();
            }
            catch(Exception e)
            {
                TemporaryLogFileScanningError(e, fileDetail.FullNameWithPath);
            }
        }

        return await SaveFileDetailsAsync(writeCount, fileDetails, cancellationToken);
    }

    private async Task<Result<bool, ErrorResponse>> SaveFileDetailsAsync(int writeCount, List<FileDetail> fileDetails,
        CancellationToken cancellationToken)
        => await Try.RunAsync(async () =>
                              {
                                  filesContext.Files.AddRange(fileDetails);
                                  _ = await filesContext.SaveChangesAsync(cancellationToken);
                                  logger.LogInformation("Saved {WriteCount} files", writeCount);

                                  return true;
                              })
                    .ToErrorResponseAsync();

    private void TemporaryLogFileScanningError(Exception e, string path) => logger.LogError(e, "An error occurred while scanning file: {FileName}", path);
}
