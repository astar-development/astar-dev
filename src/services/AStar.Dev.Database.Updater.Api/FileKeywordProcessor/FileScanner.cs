using System.Text.RegularExpressions;
using System.Threading.Channels;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public class FileScanner(IKeywordProvider keywordProvider, ChannelWriter<FileKeywordMatch> writer, ThroughputTracker tracker, ILogger<FileScanner> logger)
{
    public async Task ScanFilesAsync(IReadOnlyCollection<string> filePaths, CancellationToken cancellationToken = default)
    {
        var keywords = await keywordProvider.GetKeywordsAsync(cancellationToken);
        var pattern  = $@"\b({string.Join("|", keywords.Select(Regex.Escape))})\b";
        var regex    = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        logger.LogInformation("Scanning files for keywords");
        logger.LogInformation("File Count: {FileCount}",       filePaths.Count);
        logger.LogInformation("Keyword Count: {KeywordCount}", keywords.Count);
        logger.LogInformation("Pattern: {Pattern}",            pattern);

        await Task.Run(() => {
                           Parallel.ForEach(filePaths, new() { CancellationToken = cancellationToken },
                                            path => {
                                                var nameToCheck = path
                                                                  .Replace(Path.DirectorySeparatorChar,    ' ')
                                                                  .Replace(Path.AltDirectorySeparatorChar, ' ')
                                                                  .Replace("-",                            " ").Replace("_", " ");

                                                var matches = regex.Matches(nameToCheck)
                                                                   .Select(m => m.Value)
                                                                   .Distinct(StringComparer.OrdinalIgnoreCase);

                                                foreach(var keyword in matches)
                                                {
                                                    logger.LogInformation("Found keyword: {Keyword} in file: {FileName}", keyword, path);
                                                    writer.TryWrite(new() { FileName = path, Keyword = keyword });
                                                }

                                                tracker.RecordEvent();
                                            });
                       }, cancellationToken);

        writer.Complete();
    }
}
