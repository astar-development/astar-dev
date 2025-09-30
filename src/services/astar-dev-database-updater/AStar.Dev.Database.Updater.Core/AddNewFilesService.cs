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
using SkiaSharp;

namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     The <see cref="AddNewFilesService" /> class
/// </summary>
/// <param name="fileSystem">An instance of <see cref="IFileSystem" /> to retrieve the files from</param>
/// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
/// <param name="filesContext">An instance of the <see cref="FilesContext" /></param>
/// <param name="classificationRepository">An instance of the <see cref="ClassificationRepository" /> </param>
/// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
public class AddNewFilesService(
    IFileSystem                            fileSystem,
    IOptions<DatabaseUpdaterConfiguration> config,
    ClassificationRepository               classificationRepository,
    FilesContext                           filesContext,
    ILogger<AddNewFilesService>            logger)
{
    /// <summary>
    ///     The StartAsync method is called by the runtime (via the BackgroundWorker) and will update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    public async Task<Result<int, ErrorResponse>> StartAsync(CancellationToken stoppingToken)
    {
        var enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
        var classifications    = classificationRepository.GetExistingClassifications();

        return await GetFileList(enumerationOptions)
                     .BindAsync(async fileList => await ProcessNewFiles(fileList, classifications, stoppingToken))
                     .MapFailureAsync(x => new ErrorResponse(x.Message));
    }

    private Result<string[], ErrorResponse> GetFileList(EnumerationOptions enumerationOptions)
        => Try.Run(() => fileSystem.Directory.EnumerateFiles(config.Value.RootDirectory, "*", enumerationOptions).ToArray()).ToErrorResponse();

    private async Task<Result<int, ErrorResponse>> ProcessNewFiles(string[] files, List<FileClassification> fileClassifications, CancellationToken stoppingToken)
    {
        logger.LogInformation("Processing {FileCount} potentially new files...", files.Length);
        var count = 0;

        var fileChunks = files.Chunk(100);

        foreach(var fileChunk in fileChunks)
        {
            var first = fileChunk[0];
            logger.LogInformation("Processing potentially new file {File}...", first);
            var filesAlreadyInTheContext = await filesContext.Files.AsNoTracking().Select(f => f.FullNameWithPath).ToListAsync(stoppingToken);
            var filesToProcess           = fileChunk.Except(filesAlreadyInTheContext).ToArray();

            count = filesToProcess.Aggregate(count, (current, file) => ProcessNewFile(fileClassifications, file, current));

            await filesContext.SaveChangesAsync(stoppingToken);
        }

        return new Result<int, ErrorResponse>.Ok(count);
    }

    private int ProcessNewFile(List<FileClassification> fileClassifications, string file, int count)
    {
        var fileInfo                = fileSystem.FileInfo.New(file);
        var fileWithClassifications = UpdateFileDetailWithClassifications(fileClassifications, fileInfo, file);

        if(file.IsImage())
        {
            UpdateFileDetailsForImage(file, fileWithClassifications);
        }

        if(IsNotAnExistingFileInTheContext(fileWithClassifications))
        {
            count++;
            filesContext.Files.Add(fileWithClassifications);
        }

        var newCount = UpdateFileContext(file, count, fileWithClassifications);

        return newCount;
    }

    private int UpdateFileContext(string file, int count, FileDetail fileWithClassifications)
    {
        var newCount = 0;

        try
        {
            newCount = TryContextUpdate(file, count);
        }
        catch(DbUpdateException exception) when(exception.GetBaseException().Message.Contains("IX_FileDetail_FileHandle"))
        {
            RetryContextUpdate(fileWithClassifications);
        }

        return newCount;
    }

    private int TryContextUpdate(string file, int count)
    {
        if(count <= 100)
        {
            return count;
        }

        count = 0;

        var totalFilesCount = filesContext.Files.Count();
        logger.LogInformation("Saving {FileName} at {StartTime} (UTC)... Total Files: {TotalFilesCount}", file, DateTime.UtcNow, totalFilesCount);

        return count;
    }

    private void RetryContextUpdate(FileDetail fileWithClassifications)
    {
        try
        {
            var countFileHandle2 = filesContext.Files.Count(fileDetail => fileDetail.FileHandle == fileWithClassifications.FileHandle) + Random.Shared.Next(5_000, 500_000);
            fileWithClassifications.FileHandle = FileHandle.Create($"{countFileHandle2}-{fileWithClassifications.FileHandle}");
        }
        catch(Exception exception2)
        {
            var exToLog = new Exception(exception2.GetBaseException().Message); // full exception is too large to log "as is" - come back to this...
            logger.LogError(exToLog, "An exception occured while executing file service. Error was: {ErrorMessage} - retry failed", exToLog.Message);
        }
    }

    private static FileDetail UpdateFileDetailWithClassifications(List<FileClassification> fileClassifications, IFileInfo fileInfo, string file)
    {
        var fileWithClassifications = new FileDetail(fileInfo) { FileClassifications = [], FileAccessDetail = { DetailsLastUpdated = DateTime.UtcNow } };

        foreach(var fileClassification in GetFileClassifications(fileClassifications, file))
        {
            fileWithClassifications.FileClassifications.Add(fileClassification);
        }

        var fileClassificationsCount = fileWithClassifications.FileClassifications.Count;

        fileWithClassifications.FileHandle = GenerateFileHandle(fileInfo, fileClassificationsCount, fileWithClassifications);

        return fileWithClassifications;
    }

    private static IEnumerable<FileClassification> GetFileClassifications(List<FileClassification> fileClassifications, string file)
        => from fileClassification in fileClassifications
           from fileNamePart in fileClassification.FileNameParts
           where file.Contains(fileNamePart.Text)
           select fileClassification;

    private bool IsNotAnExistingFileInTheContext(FileDetail fileWithClassifications) =>
        !filesContext.Files.Any(f => f.DirectoryName == fileWithClassifications.DirectoryName && f.FileName == fileWithClassifications.FileName);

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

    private static FileHandle GenerateFileHandle(IFileInfo fileInfo, int fileClassificationsCount, FileDetail fileWithClassifications)
    {
        var fileHandle = $"{GenerateRandomNumeric()}-{GenerateFileHandle(fileInfo.Name).Value}";

        if(fileClassificationsCount <= 1)
        {
            return FileHandle.Create(fileHandle.TruncateIfRequired(350));
        }

        var firstRandom  = new Random(DateTime.Now.Millisecond).Next(1, fileClassificationsCount);
        var prefix       = fileWithClassifications.FileClassifications.Skip(firstRandom).FirstOrDefault()?.FileNameParts.FirstOrDefault()?.Text;
        var secondRandom = new Random(DateTime.Now.Millisecond).Next(1, fileClassificationsCount);
        var prefix2      = fileWithClassifications.FileClassifications.Skip(secondRandom).FirstOrDefault()?.FileNameParts.FirstOrDefault()?.Text;

        fileHandle = UpdateFileHandle(fileInfo, prefix, prefix2);

        return GenerateFileHandle(fileHandle.TruncateIfRequired(350));
    }

    private static string UpdateFileHandle(IFileInfo fileInfo, string? prefix, string? prefix2)
        => prefix switch
           {
               { Length: > 0 } when prefix2 is { Length: > 0 } && prefix != prefix2 => GenerateHandleWithTwoPrefixes(fileInfo, prefix, prefix2),
               { Length: > 0 }                                                      => GenerateHandleWithOnePrefix(fileInfo, prefix),
               _                                                                    => GenerateHandleWithoutPrefixes(fileInfo)
           };

    private static string GenerateHandleWithoutPrefixes(IFileInfo fileInfo) => $"{GenerateRandomNumeric()}-{GenerateFileHandle(fileInfo.Name)}";

    private static string GenerateHandleWithOnePrefix(IFileInfo fileInfo, string prefix) => $"{GenerateRandomNumeric()}-{prefix}-{GenerateFileHandle(fileInfo.Name)}";

    private static string GenerateHandleWithTwoPrefixes(IFileInfo fileInfo, string prefix, string prefix2) => $"{GenerateRandomNumeric()}-{prefix}-{prefix2}-{GenerateFileHandle(fileInfo.Name)}";

    private static int GenerateRandomNumeric() => Random.Shared.Next(5_000, 500_000);

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
