using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace AStar.Dev.Database.Updater.FileKeywordProcessor;

public class FileScanner(ThroughputTracker tracker)
{
    public async Task ScanFilesAsync(IEnumerable<string> keywords, IEnumerable<string> filePaths, ChannelWriter<FileKeywordMatch> writer, CancellationToken cancellationToken = default)
    {
        var pattern = $@"\b({string.Join("|", keywords.Select(Regex.Escape))})\b";
        var regex   = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        await Task.Run(() => {
                           Parallel.ForEach(filePaths, new() { CancellationToken = cancellationToken },
                                            path => {
                                                var nameToCheck = Path.GetFileNameWithoutExtension(path) ?? string.Empty;

                                                if(string.IsNullOrEmpty(nameToCheck))
                                                {
                                                    return;
                                                }

                                                var matches = regex.Matches(nameToCheck)
                                                                   .Select(m => m.Value)
                                                                   .Distinct(StringComparer.OrdinalIgnoreCase);

                                                foreach(var keyword in matches)
                                                {
                                                    writer.TryWrite(new() { FileName = path, Keyword = keyword });
                                                }

                                                // Record throughput event (per file scanned)
                                                tracker.RecordEvent();
                                            });
                       }, cancellationToken);

        writer.Complete();
    }
}
