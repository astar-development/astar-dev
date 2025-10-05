using System.Text.RegularExpressions;
using System.Threading.Channels;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="keywordProvider"></param>
/// <param name="writer"></param>
/// <param name="tracker"></param>
/// <param name="logger"></param>
public class FileScanner(IKeywordProvider keywordProvider, ChannelWriter<FileKeywordMatch> writer, ThroughputTracker tracker, ILogger<FileScanner> logger)
{
    /// <summary>
    /// </summary>
    /// <param name="filePaths"></param>
    /// <param name="cancellationToken"></param>
    public async Task ScanFilesAsync(IReadOnlyCollection<string> filePaths, CancellationToken cancellationToken = default)
    {
        var counter  = 0;
        var keywords = await keywordProvider.GetKeywordsAsync(cancellationToken);
        var pattern  = $@"\b({string.Join("|", keywords.Select(Regex.Escape))})\b";
        var regex    = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        logger.LogInformation("Scanning files for keywords in: File Count: {FileCount} with Keyword Count: {KeywordCount}", filePaths.Count, keywords.Count);

        await Task.Run(() =>
                       {
                           Parallel.ForEach(filePaths, new() { CancellationToken = cancellationToken },
                                            path =>
                                            {
                                                var nameToCheck = path
                                                                  .Replace(Path.DirectorySeparatorChar,    ' ')
                                                                  .Replace(Path.AltDirectorySeparatorChar, ' ')
                                                                  .Replace("-",                            " ").Replace("_", " ");

                                                var matches = regex.Matches(nameToCheck)
                                                                   .Select(m => m.Value)
                                                                   .Distinct(StringComparer.OrdinalIgnoreCase);

                                                foreach(var keyword in matches.Where(m => m.Length > 0))
                                                {
                                                    counter++;

                                                    if(counter % 1000 == 0)
                                                    {
                                                        logger.LogInformation("Found keyword: {Keyword} in file: {FileName}", keyword, path);
                                                    }

                                                    writer.TryWrite(new() { FileName = path, Keyword = keyword });
                                                }

                                                tracker.RecordEvent();
                                            });
                       }, cancellationToken);

        writer.Complete();
    }
}
