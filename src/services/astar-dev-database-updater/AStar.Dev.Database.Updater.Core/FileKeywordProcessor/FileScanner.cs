using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="classificationRepository"></param>
/// <param name="keywordProvider"></param>
/// <param name="writer"></param>
/// <param name="tracker"></param>
/// <param name="logger"></param>
public class FileScanner(
    IServiceScopeFactory      serviceScopeFactory,
    ClassificationRepository  classificationRepository,
    IKeywordProvider          keywordProvider,
    ChannelWriter<FileDetail> writer,
    ThroughputTracker         tracker,
    ILogger<FileScanner>      logger)
{
    /// <summary>
    /// </summary>
    /// <param name="filesToProcess"></param>
    /// <param name="cancellationToken"></param>
    public async Task ScanFilesAsync(List<FileDetail> filesToProcess, CancellationToken cancellationToken = default)
    {
        var counter  = 0;
        var keywords = await keywordProvider.GetKeywordsAsync(cancellationToken);
        var pattern  = KeywordRegexBuilder.BuildKeywordPattern(keywords);
        var regex    = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        logger.LogInformation("Scanning files for keywords in: File Count: {FileCount} with Keyword Count: {KeywordCount}", filesToProcess.Count, keywords.Count);
    using var scope                          = serviceScopeFactory.CreateScope();
    var       filesContext                   = scope.ServiceProvider.GetRequiredService<FilesContext>();
    var       fileHandlesAlreadyInTheContext = await filesContext.Files.Select(f => f.FileHandle).ToListAsync(cancellationToken);

    // Load classifications from the repository but re-query them from the same FilesContext
    // so the returned entities are tracked by the context we'll use to save FileDetails.
    var classificationsFromRepo = classificationRepository.GetExistingClassifications();
    var classificationIds       = classificationsFromRepo.Select(c => c.Id).ToList();
    var classifications         = await filesContext.FileClassifications
                               .Include(fc => fc.FileNameParts)
                               .Where(fc => classificationIds.Contains(fc.Id))
                               .ToListAsync(cancellationToken);
        // Ensure logs directory exists when running under test environment
        try
        {
            _ = Directory.CreateDirectory("logs");
        }
        catch
        {
            // If creation fails, proceed — File.Delete below will be guarded by existence checks.
        }

        if(File.Exists("logs/progress.log.txt"))
        {
            File.Delete("logs/progress.log.txt");
        }

        if(File.Exists("logs/error.log.txt"))
        {
            File.Delete("logs/error.log.txt");
        }
    var              writeCount  = 0;
    List<FileDetail> fileDetails = new();

        foreach(var fileDetail in filesToProcess)
        {
            if(cancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                var nameToCheck = SanitizeFilePath(fileDetail.FullNameWithPath);

                var matches = GetFilenameMatches(regex, nameToCheck);

                var nonEmptyMatches = matches.Where(m => m.Length > 0).ToArray();

                counter = ProcessMatches(nonEmptyMatches, counter, classifications, fileDetail);

                fileDetail.FileHandle = GenerateFileHandle(fileDetail, fileHandlesAlreadyInTheContext);

                if(fileDetail.FileName.Value.IsImage())
                {
                    UpdateFileDetailsForImage(fileDetail);
                }

                //writer.TryWrite(fileDetail);
                fileDetails.Add(fileDetail);
                tracker.RecordEvent();
                writeCount++;

                if(writeCount % 100 == 0)
                {

                    filesContext.Files.AddRange(fileDetails);

                    try
                    {
                        _ = await filesContext.SaveChangesAsync(cancellationToken);
                    }
                    catch(Exception e)
                    {
                        TemporaryLogFileScanningError(e, "Save Changes");
                    }

                    fileDetails                    = new();
                    fileHandlesAlreadyInTheContext = filesContext.Files.Select(f => f.FileHandle).ToList();

                    await File.AppendAllTextAsync("logs/progress.log.txt", $"{DateTimeOffset.UtcNow} - {fileDetail}{Environment.NewLine}", cancellationToken);
                }
            }
            catch(Exception e)
            {
                TemporaryLogFileScanningError(e, fileDetail.FullNameWithPath);
            }
        }

        // Persist any remaining file details that didn't trigger the batch save (for small runs)
        if(fileDetails.Count > 0)
        {
            filesContext.Files.AddRange(fileDetails);

            try
            {
                _ = await filesContext.SaveChangesAsync(cancellationToken);
            }
            catch(Exception e)
            {
                TemporaryLogFileScanningError(e, "Final Save Changes");
            }
        }

        logger.LogInformation("Closing the writer");

        writer.Complete();
    }

        // ...existing code...


    private void TemporaryLogFileScanningError(Exception e, string path)
    {
        logger.LogError(e, "An error occured while scanning file: {FileName}", path);
        Console.WriteLine(new string('-', 100));
        logger.LogWarning(new('-', 100));
        Console.WriteLine(e);
        File.AppendAllText("logs/error.log.txt", e + Environment.NewLine);
    }

    private int ProcessMatches(string[] nonEmptyMatches, int counter, List<FileClassification> classifications, FileDetail fileWithClassifications)
    {
        foreach(var keyword in nonEmptyMatches)
        {
            counter++;

            if(counter % 10 == 0)
            {
                logger.LogInformation("Found keyword: {Keyword} in file: {FileName}", keyword, fileWithClassifications.FullNameWithPath);
            }

            foreach(var fileClassification in GetFileClassifications(classifications, fileWithClassifications.FullNameWithPath))
            {
                fileWithClassifications.FileClassifications.Add(fileClassification);
            }
        }

        return counter;
    }

    private string[] GetFilenameMatches(Regex regex, string nameToCheck)
    {
        var matches = regex.Matches(nameToCheck)
                           .Select(m => m.Value)
                           .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        return matches;
    }

    private static string SanitizeFilePath(string path) => path
                                                           .Replace(Path.DirectorySeparatorChar,    ' ')
                                                           .Replace(Path.AltDirectorySeparatorChar, ' ')
                                                           .Replace('-',                            ' ')
                                                           .Replace('_',                            ' ');

    private static IEnumerable<FileClassification> GetFileClassifications(List<FileClassification> fileClassifications, string file)
        => from fileClassification in fileClassifications
           from fileNamePart in fileClassification.FileNameParts
           where file.Contains(fileNamePart.Text)
           select fileClassification;

    private static FileHandle GenerateFileHandle(FileDetail fileInfo, List<FileHandle> fileHandlesAlreadyInTheContext)
    {
        var fileHandle = GenerateFileHandle(fileInfo.FileName.Value).Value.TruncateIfRequired(350);

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

    private void UpdateFileDetailsForImage(FileDetail fileDetail)
    {
        try
        {
            fileDetail.IsImage = true;
            var image = SKImage.FromEncodedData(fileDetail.FullNameWithPath);

            if(image is null)
            {
                return;
            }

            fileDetail.ImageDetail = new(image.Width, image.Height);
        }
        catch(Exception e)
        {
            Console.WriteLine(e);

            TemporaryLogFileScanningError(e, fileDetail.FullNameWithPath);
        }
    }
}
