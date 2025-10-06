using System.Globalization;
using System.IO.Abstractions;
using System.Text.RegularExpressions;
using System.Threading.Channels;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="fileSystem">An instance of <see cref="IFileSystem" /> to retrieve the files from</param>
/// <param name="filesContext"></param>
/// <param name="classificationRepository"></param>
/// <param name="keywordProvider"></param>
/// <param name="writer"></param>
/// <param name="tracker"></param>
/// <param name="logger"></param>
public class FileScanner(
    IFileSystem               fileSystem,
    FilesContext              filesContext,
    ClassificationRepository  classificationRepository,
    IKeywordProvider          keywordProvider,
    ChannelWriter<FileDetail> writer,
    ThroughputTracker         tracker,
    ILogger<FileScanner>      logger)
{
    /// <summary>
    /// </summary>
    /// <param name="filePaths"></param>
    /// <param name="cancellationToken"></param>
    public async Task ScanFilesAsync(IReadOnlyCollection<string> filePaths, CancellationToken cancellationToken = default)
    {
        var counter  = 0;
        var keywords = await keywordProvider.GetKeywordsAsync(cancellationToken);
        var pattern  = KeywordRegexBuilder.BuildKeywordPattern(keywords);
        var regex    = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        logger.LogInformation("Scanning files for keywords in: File Count: {FileCount} with Keyword Count: {KeywordCount}", filePaths.Count, keywords.Count);
        var fileHandlesAlreadyInTheContext = await filesContext.Files.AsNoTracking().Select(f => f.FileHandle).ToListAsync(cancellationToken);
        var classifications                = classificationRepository.GetExistingClassifications();
        var filesAlreadyInTheContext       = await filesContext.Files.AsNoTracking().Select(f => f.FullNameWithPath).ToListAsync(cancellationToken);
        var filesToProcess                 = filePaths.Except(filesAlreadyInTheContext).ToArray();
        File.WriteAllLines("logs/files-to-process.txt", filesToProcess);
        logger.LogInformation("Starting scanning files");
        logger.LogInformation("Found {FileCount} files to process", filesToProcess.Length);

        await Task.Run(() =>
                       {
                           logger.LogInformation("Starting scanning files - in parallel");

                           Parallel.ForEach(filesToProcess, new() { CancellationToken = cancellationToken },
                                            path =>
                                            {
                                                try
                                                {
                                                    var nameToCheck = SanitizeFilePath(path);

                                                    var matches  = GetFilenameMatches(regex, nameToCheck, path);
                                                    var fileInfo = fileSystem.FileInfo.New(path);

                                                    var fileWithClassifications = new FileDetail(fileInfo) { Id = new() { Value = Guid.CreateVersion7() } };

                                                    var nonEmptyMatches = matches.Where(m => m.Length > 0).ToArray();

                                                    logger.LogInformation("Found {MatchCount} matches in file: {FileName}", nonEmptyMatches.Length, path);
                                                    counter = ProcessMatches(nonEmptyMatches, counter, path, classifications, fileWithClassifications);

                                                    fileWithClassifications.FileHandle = GenerateFileHandle(fileInfo, fileHandlesAlreadyInTheContext);

                                                    if(fileWithClassifications.FileName.Value.IsImage())
                                                    {
                                                        UpdateFileDetailsForImage(fileWithClassifications);
                                                    }

                                                    writer.TryWrite(fileWithClassifications);
                                                    logger.LogInformation("Finished scanning file: {FileName}", path);

                                                    tracker.RecordEvent();
                                                }
                                                catch(Exception e)
                                                {
                                                    TemporaryLogFileScanningError(e, path);
                                                }
                                            });

                           logger.LogInformation("Finished scanning files - in parallel");
                       }, cancellationToken);

        logger.LogInformation("Closing the writer");

        writer.Complete();
    }

    private void TemporaryLogFileScanningError(Exception e, string path)
    {
        logger.LogError(e, "An error occured while scanning file: {FileName}", path);
        Console.WriteLine(new string('-', 100));
        logger.LogWarning(new('-', 100));
        Console.WriteLine(e);
        File.AppendAllText("logs/error.log.txt", e + Environment.NewLine);
        ;
    }

    private int ProcessMatches(string[] nonEmptyMatches, int counter, string path, List<FileClassification> classifications, FileDetail fileWithClassifications)
    {
        foreach(var keyword in nonEmptyMatches)
        {
            counter++;

            if(counter % 1000 == 0)
            {
                logger.LogInformation("Found keyword: {Keyword} in file: {FileName}", keyword, path);
            }

            foreach(var fileClassification in GetFileClassifications(classifications, path))
            {
                fileWithClassifications.FileClassifications.Add(fileClassification);
            }
        }

        return counter;
    }

    private string[] GetFilenameMatches(Regex regex, string nameToCheck, string path)
    {
        var matches = regex.Matches(nameToCheck)
                           .Select(m => m.Value)
                           .Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

        logger.LogInformation("Starting scanning file: {FileName}", path);
        logger.LogInformation("Found {MatchCount} matches",         matches.Length);

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
