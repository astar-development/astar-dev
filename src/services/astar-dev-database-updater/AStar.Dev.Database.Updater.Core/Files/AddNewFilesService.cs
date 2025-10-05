using System.Globalization;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Functional.Extensions;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using SkiaSharp;

namespace AStar.Dev.Database.Updater.Core.Files;

/// <summary>
///     The <see cref="AddNewFilesService" /> class
/// </summary>
/// <param name="fileSystem">An instance of <see cref="IFileSystem" /> to retrieve the files from</param>
/// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
/// <param name="filesContext">An instance of the <see cref="FilesContext" /></param>
/// <param name="classificationRepository">An instance of the <see cref="ClassificationRepository" /> </param>
/// <param name="logger">An instance of the <see cref="Microsoft.Extensions.Logging.ILogger" /> to log status / errors</param>
public class AddNewFilesService(
    IFileSystem                            fileSystem,
    IOptions<DatabaseUpdaterConfiguration> config,
    ClassificationRepository               classificationRepository,
    FilesContext                           filesContext,
    ILogger<AddNewFilesService>            logger)
{
    /// <summary>
    ///     The FindNewFilesAndAddToDatabaseAsync method is called to update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    public async Task<Result<int, ErrorResponse>> FindNewFilesAndAddToDatabaseAsync(CancellationToken stoppingToken)
    {
        var enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
        var classifications    = classificationRepository.GetExistingClassifications();

        return await GetFileList(enumerationOptions)
                     .BindAsync(async fileList => await ProcessNewFilesAsync(fileList, classifications, stoppingToken))
                     .MapFailureAsync(x => new ErrorResponse(x.Message));
    }

    private Result<string[], ErrorResponse> GetFileList(EnumerationOptions enumerationOptions)
        => Try.Run(() => fileSystem.Directory.EnumerateFiles(config.Value.RootDirectory, "*", enumerationOptions).ToArray()).ToErrorResponse();

    private async Task<Result<int, ErrorResponse>> ProcessNewFilesAsync(string[] files, List<FileClassification> fileClassifications, CancellationToken stoppingToken)
    {
        logger.LogInformation("Processing {FileCount} potentially new files...", files.Length);
        var count = 0;

        var fileChunks                     = files.Chunk(100);
        var filesAlreadyInTheContext       = await filesContext.Files.AsNoTracking().Select(f => f.FullNameWithPath).ToListAsync(stoppingToken);
        var fileHandlesAlreadyInTheContext = await filesContext.Files.AsNoTracking().Select(f => f.FileHandle).ToListAsync(stoppingToken);

        foreach(var fileList in fileChunks)
        {
            logger.LogInformation("Processing potentially new file {File}...", fileList[0]);
            var filesToProcess = fileList.Except(filesAlreadyInTheContext).ToArray();

            foreach(var file in filesToProcess)
            {
                count = await ProcessNewFileAsync(fileClassifications, file, count, fileHandlesAlreadyInTheContext, stoppingToken);

                return new Result<int, ErrorResponse>.Ok(count);
            }
        }

        await filesContext.SaveChangesAsync(stoppingToken);

        return new Result<int, ErrorResponse>.Ok(count);
    }

    private async Task<int> ProcessNewFileAsync(List<FileClassification> fileClassifications, string file, int count, List<FileHandle> fileHandlesAlreadyInTheContext, CancellationToken stoppingToken)
    {
        var fileInfo                = fileSystem.FileInfo.New(file);
        var fileWithClassifications = UpdateFileDetailWithClassifications(fileClassifications, fileInfo, file, fileHandlesAlreadyInTheContext);

        if(file.IsImage())
        {
            UpdateFileDetailsForImage(file, fileWithClassifications);
        }

        count++;
        filesContext.Files.Add(fileWithClassifications);

        var newCount = await UpdateFileContextAsync(count, fileWithClassifications, stoppingToken);

        return newCount;
    }

    private async Task<int> UpdateFileContextAsync(int count, FileDetail fileWithClassifications, CancellationToken stoppingToken)
    {
        var newCount = 0;

        try
        {
            newCount = await TryContextUpdateAsync(count, stoppingToken);
        }
        catch(DbUpdateException exception) when(exception.GetBaseException().Message.Contains("IX_FileDetail_FileHandle"))
        {
            RetryContextUpdate(fileWithClassifications);
        }

        return newCount;
    }

    private async Task<int> TryContextUpdateAsync(int count, CancellationToken stoppingToken)
    {
        if(count <= 100)
        {
            return count;
        }

        count = 0;
        await filesContext.SaveChangesAsync(stoppingToken);

        return count;
    }

    private void RetryContextUpdate(FileDetail fileWithClassifications)
    {
        try
        {
            Log.Warning("File handle collision detected. Retrying...");
            fileWithClassifications.FileHandle = FileHandle.Create($"{Guid.CreateVersion7()}-{fileWithClassifications.FileHandle}".TruncateIfRequired(350));
        }
        catch(Exception exception2)
        {
            var exToLog = new Exception(exception2.GetBaseException().Message); // the full exception is too large to log "as is" - come back to this...
            logger.LogError(exToLog, "An exception occured while executing file service. Error was: {ErrorMessage} - retry failed", exToLog.Message);
        }
    }

    private static FileDetail UpdateFileDetailWithClassifications(List<FileClassification> fileClassifications, IFileInfo fileInfo, string file, List<FileHandle> fileHandlesAlreadyInTheContext)
    {
        var fileWithClassifications = new FileDetail(fileInfo) { FileClassifications = [], FileAccessDetail = { DetailsLastUpdated = DateTime.UtcNow } };

        foreach(var fileClassification in GetFileClassifications(fileClassifications, file))
        {
            fileWithClassifications.FileClassifications.Add(fileClassification);
        }

        fileWithClassifications.FileHandle = GenerateFileHandle(fileInfo, fileHandlesAlreadyInTheContext);

        return fileWithClassifications;
    }

    private static IEnumerable<FileClassification> GetFileClassifications(List<FileClassification> fileClassifications, string file)
        => from fileClassification in fileClassifications
           from fileNamePart in fileClassification.FileNameParts
           where file.Contains(fileNamePart.Text)
           select fileClassification;

    private static void UpdateFileDetailsForImage(string file, FileDetail fileDetail)
    {
        fileDetail.IsImage = true;
        var image = SKImage.FromEncodedData(file);

        if(image is null)
        {
            return;
        }

        fileDetail.ImageDetail = new(image.Width, image.Height);
    }

    private static FileHandle GenerateFileHandle(IFileInfo fileInfo, List<FileHandle> fileHandlesAlreadyInTheContext)
    {
        var fileHandle = GenerateFileHandle(fileInfo.Name).Value.TruncateIfRequired(350);

        if(fileHandlesAlreadyInTheContext.Any(h => h.Value == fileHandle))
        {
            fileHandle = $"{Guid.CreateVersion7()}-{fileHandle}".TruncateIfRequired(350);
        }

        var newHandle = FileHandle.Create(fileHandle);
        fileHandlesAlreadyInTheContext.Add(newHandle);

        return newHandle;
    }

    private static FileHandle GenerateFileHandle(string file)
    {
        var newHandle = Regex.Replace(Path.GetInvalidPathChars()
                                          .Aggregate(file, (current, illegalCharacter) => current.Replace(illegalCharacter, '-'))
                                          .Replace("/", "-")
                                          .Replace(" ", "-")
                                          .Replace("_", "-")
                                          .Replace("(", "-")
                                          .Replace(")", "-")
                                          .Replace("+", "-")
                                          .Replace(".", "-")
                                          .Replace(",", "-")
                                          .ToLower(CultureInfo.InvariantCulture)
                                          .Replace("sammy", "sam"), @"(-+)", "-", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

        return FileHandle.Create(newHandle.StartsWith("-", StringComparison.OrdinalIgnoreCase)
                                     ? newHandle[1..]
                                     : newHandle);
    }
}
