using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class MetricsCollector
{
    private readonly Stopwatch _stopwatch = new();
    private int _filesDownloaded;
    private long _bytesDownloaded;

    public void Start() => _stopwatch.Start();

    public void RecordFile(long bytes)
    {
        _ = Interlocked.Increment(ref _filesDownloaded);
        _ = Interlocked.Add(ref _bytesDownloaded, bytes);
    }

    public string GetSummary()
    {
        TimeSpan elapsed = _stopwatch.Elapsed;
        var seconds = elapsed.TotalSeconds > 0 ? elapsed.TotalSeconds : 1;

        var filesPerSec = _filesDownloaded / seconds;
        var mbPerSec = _bytesDownloaded / 1024.0 / 1024.0 / seconds;

        return $"📊 Files: {_filesDownloaded}, " +
               $"Data: {_bytesDownloaded / 1024.0 / 1024.0:F2} MB, " +
               $"Elapsed: {elapsed:mm\\:ss}, " +
               $"Rate: {filesPerSec:F2} files/s, {mbPerSec:F2} MB/s";
    }
}

public sealed class FolderMetricsCollector
{
    private readonly ConcurrentDictionary<string, FolderStats> _folderStats = new();

    public void RecordFile(string folderPath, long bytes)
    {
        FolderStats stats = _folderStats.GetOrAdd(folderPath, _ => new FolderStats());
        stats.AddFile(bytes);
    }

    public IEnumerable<(string Folder, int Files, double MB)> GetSummary()
    {
        foreach(KeyValuePair<string, FolderStats> kvp in _folderStats)
        {
            yield return (kvp.Key, kvp.Value.FileCount, kvp.Value.TotalBytes / 1024.0 / 1024.0);
        }
    }

    private sealed class FolderStats
    {
        private int _fileCount;
        private long _totalBytes;

        public int FileCount => _fileCount;
        public long TotalBytes => _totalBytes;

        public void AddFile(long bytes)
        {
            _ = Interlocked.Increment(ref _fileCount);
            _ = Interlocked.Add(ref _totalBytes, bytes);
        }
    }
}
