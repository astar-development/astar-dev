using System.Globalization;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using AStar.Dev.Files.Api.Client.Sdk.FilesApi;
using AStar.Dev.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core;

/// <summary>
///     The <see cref="AddNewFilesService" /> class
/// </summary>
/// <param name="filesApiClient">The <see cref="FilesApiClient" /> required by the AddNewFilesBackgroundService</param>
/// <param name="fileSystem">An instance of <see cref="IFileSystem" /> to retrieve the files from</param>
/// <param name="config">An instance of the <see cref="DatabaseUpdaterConfiguration" /> options used to configure the addition of the new files</param>
/// <param name="logger">An instance of the <see cref="ILogger" /> to log status / errors</param>
public class AddNewFilesService(FilesApiClient filesApiClient, IFileSystem fileSystem, IOptions<DatabaseUpdaterConfiguration> config, ILogger<AddNewFilesService> logger)
{
    /// <summary>
    ///     The StartAsync method is called by the runtime and will update the database with any new files
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    public async Task StartAsync(CancellationToken stoppingToken)
    {
        var enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };

        var files = fileSystem.Directory.EnumerateFiles(config.Value.RootDirectory, "*", enumerationOptions).ToArray();

        logger.LogInformation("Processing {FileCount} potentially new files...", files.Length);

        // var fileClassifications = filesApiClient.FileClassifications.Include(f => f.FileNameParts).ToList();
        //
        // await ProcessNewFiles(files, fileClassifications, stoppingToken);
        filesApiClient.ToJson();
        await Task.CompletedTask;
    }

    //
    // private async Task ProcessNewFiles(string[] files, List<FileClassification> fileClassifications, CancellationToken stoppingToken)
    // {
    //     var filesAlreadyInTheContext = await filesApiClient.Files.Select(f => f.FullNameWithPath).ToListAsync(stoppingToken);
    //     var filesToProcess           = files.Except(filesAlreadyInTheContext).ToArray();
    //
    //     if(filesToProcess.Length == 0)
    //     {
    //         logger.LogInformation("No new files to process...exiting...");
    //
    //         return;
    //     }
    //
    //     logger.LogInformation("Processing {NewFiles} new files...", filesToProcess.Length);
    //
    //     var count            = 0;
    //
    //     foreach (var file in filesToProcess)
    //     {
    //         count = await ProcessNewFile(fileClassifications, file, count, stoppingToken);
    //     }
    // }
    //
    // private async Task<int> ProcessNewFile(   List<FileClassification> fileClassifications, string file, int count, CancellationToken stoppingToken)
    // {
    //     var fileInfo                = fileSystem.FileInfo.New(file);
    //     var fileWithClassifications = UpdateFileDetailWithClassifications(fileClassifications, fileInfo, file);
    //
    //     if (file.IsImage())
    //     {
    //         UpdateFileDetailsForImage(file, fileWithClassifications);
    //     }
    //
    //     if (IsNotAnExistingFileInTheContext(fileWithClassifications))
    //     {
    //         count++;
    //         filesApiClient.Files.Add(fileWithClassifications);
    //     }
    //
    //     var newCount = await UpdateFileContext(file, count, fileWithClassifications, stoppingToken);
    //
    //     return newCount;
    // }
    //
    // private async Task<int> UpdateFileContext(   string file, int count, FileDetail fileWithClassifications, CancellationToken stoppingToken)
    // {
    //     var newCount = 0;
    //
    //     try
    //     {
    //         newCount = await TryContextUpdate( file, count, stoppingToken);
    //     }
    //     catch (DbUpdateException exception) when (exception.GetBaseException().Message.Contains("IX_FileDetail_FileHandle") )
    //     {
    //         await RetryContextUpdate( fileWithClassifications, stoppingToken);
    //     }
    //
    //     return newCount;
    // }
    //
    // private async Task<int> TryContextUpdate(  string file, int count, CancellationToken stoppingToken)
    // {
    //     await filesApiClient.SaveChangesAsync(stoppingToken);
    //
    //     if (count <= 100)
    //     {
    //         return count;
    //     }
    //
    //     count = 0;
    //
    //     await filesApiClient.SaveChangesAsync(stoppingToken);
    //     var totalFilesCount = filesApiClient.Files.Count();
    //     logger.LogInformation("Saving {FileName} at {StartTime} (UTC)... Total Files: {TotalFilesCount}", file, DateTime.UtcNow, totalFilesCount);
    //
    //     return count;
    // }
    //
    // private async Task RetryContextUpdate( FileDetail fileWithClassifications, CancellationToken stoppingToken)
    // {
    //     try
    //     {
    //         var countFileHandle2 = filesApiClient.Files.Count(fileDetail => fileDetail.FileHandle == fileWithClassifications.FileHandle) + Random.Shared.Next(5_000, 500_000);
    //         fileWithClassifications.FileHandle = $"{countFileHandle2}-{fileWithClassifications.FileHandle}";
    //         await filesApiClient.SaveChangesAsync(stoppingToken);
    //     }
    //     catch (Exception exception2)
    //     {
    //         var exToLog = new Exception(exception2.GetBaseException().Message); // full exception is too large to log "as is" - come back to this...
    //         logger.LogError(exToLog, "An exception occured while executing file service. Error was: {ErrorMessage} - retry failed", exToLog.Message);
    //     }
    // }
    //
    // private static FileDetail UpdateFileDetailWithClassifications(List<FileClassification> fileClassifications, IFileInfo fileInfo, string file)
    // {
    //     var fileWithClassifications = new FileDetail(fileInfo) { FileClassifications = [], FileAccessDetail    = { DetailsLastUpdated = DateTime.UtcNow } };
    //
    //     foreach (var fileClassification in GetFileClassifications(fileClassifications, file))
    //     {
    //         fileWithClassifications.FileClassifications.Add(fileClassification);
    //     }
    //
    //     var fileClassificationsCount = fileWithClassifications.FileClassifications.Count;
    //
    //     fileWithClassifications.FileHandle = GenerateFileHandle(fileInfo, fileClassificationsCount, fileWithClassifications);
    //
    //     return fileWithClassifications;
    // }
    //
    // private static IEnumerable<FileClassification> GetFileClassifications(List<FileClassification> fileClassifications, string file)
    //     => from fileClassification in fileClassifications
    //        from fileNamePart in fileClassification.FileNameParts
    //        where file.Contains(fileNamePart.Text)
    //        select fileClassification;
    //
    // private bool IsNotAnExistingFileInTheContext(FileDetail fileWithClassifications) =>
    //     !filesApiClient.Files.Any(f => f.DirectoryName == fileWithClassifications.DirectoryName && f.FileName == fileWithClassifications.FileName);
    //
    // private static void UpdateFileDetailsForImage(string file, FileDetail fileWithClassifications)
    // {
    //     fileWithClassifications.IsImage = true;
    //     var image = SKImage.FromEncodedData(file);
    //
    //     if (image is null)
    //     {
    //         return;
    //     }
    //
    //     fileWithClassifications.Height = image.Height;
    //     fileWithClassifications.Width  = image.Width;
    // }

    // private static string GenerateFileHandle(IFileInfo fileInfo, int fileClassificationsCount, FileDetail fileWithClassifications)
    // {
    //     var fileHandle = $"{GenerateRandomNumeric()}-{GenerateFileHandle(fileInfo.Name)}";
    //
    //     if (fileClassificationsCount <= 1)
    //     {
    //         return GenerateFileHandle(fileHandle.TruncateIfRequired(350));
    //     }
    //
    //     var firstRandom  = new Random(DateTime.Now.Millisecond).Next(1, fileClassificationsCount);
    //     var prefix       = fileWithClassifications.FileClassifications.Skip(firstRandom).FirstOrDefault()?.FileNameParts.FirstOrDefault()?.Text;
    //     var secondRandom = new Random(DateTime.Now.Millisecond).Next(1, fileClassificationsCount);
    //     var prefix2      = fileWithClassifications.FileClassifications.Skip(secondRandom).FirstOrDefault()?.FileNameParts.FirstOrDefault()?.Text;
    //
    //     fileHandle = UpdateFileHandle(fileInfo, prefix, prefix2);
    //
    //     return GenerateFileHandle(fileHandle.TruncateIfRequired(350));
    // }

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

    private static string GenerateFileHandle(string file)
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

        return newHandle.StartsWith("-", StringComparison.OrdinalIgnoreCase)
                   ? newHandle[1..]
                   : newHandle;
    }
}
