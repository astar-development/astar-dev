using System.Collections.Concurrent;
using System.Diagnostics;

namespace AStar.Dev.OneDrive.Client.Login;

public sealed class MetricsCollector
{
    private readonly ConcurrentDictionary<string, FolderStats> _folderStats = new();

    // Rolling window for ETA smoothing
    private readonly ConcurrentQueue<double> _recentDurations = new();
    private readonly Stopwatch _stopwatch = new();
    private readonly int _windowSize = 20; // average over last 20 files
    private long _bytesDownloaded;
    private int _errors;
    private int _filesDownloaded;
    private int _totalFiles;

    public void Start(int totalFiles)
    {
        _totalFiles = totalFiles;
        _stopwatch.Start();
    }

    public void RecordFile(string folderPath, long bytes)
    {
        _ = Interlocked.Increment(ref _filesDownloaded);
        _ = Interlocked.Add(ref _bytesDownloaded, bytes);

        FolderStats stats = _folderStats.GetOrAdd(folderPath, _ => new FolderStats());
        stats.AddFile(bytes);

        // Record duration per file
        var secondsPerFile = _stopwatch.Elapsed.TotalSeconds / _filesDownloaded;
        _recentDurations.Enqueue(secondsPerFile);
        if (_recentDurations.Count > _windowSize)
            _ = _recentDurations.TryDequeue(out _);
    }

    public void RecordError() => _ = Interlocked.Increment(ref _errors);

    public string GetGlobalSummary()
    {
        TimeSpan elapsed = _stopwatch.Elapsed;
        var seconds = elapsed.TotalSeconds > 0 ? elapsed.TotalSeconds : 1;

        var filesPerSec = _filesDownloaded / seconds;
        var mbPerSec = _bytesDownloaded / 1024.0 / 1024.0 / seconds;
        var percent = _totalFiles > 0 ? 100.0 * _filesDownloaded / _totalFiles : 0;

        // Smoothed ETA using moving average
        var avgSecondsPerFile = _recentDurations.Count > 0
            ? _recentDurations.Average()
            : seconds / Math.Max(_filesDownloaded, 1);

        double remainingFiles = _totalFiles - _filesDownloaded;
        var etaSeconds = avgSecondsPerFile * remainingFiles;
        var eta = TimeSpan.FromSeconds(etaSeconds);

        // Confidence indicator
        var confidence = "unknown";
        if (_recentDurations.Count > 5)
        {
            var mean = avgSecondsPerFile;
            var variance = _recentDurations.Select(d => Math.Pow(d - mean, 2)).Average();
            var stdDev = Math.Sqrt(variance);
            confidence = stdDev / mean < 0.25 ? "stable" : "volatile";
        }

        return $"📊 {percent:F1}% complete ({_filesDownloaded}/{_totalFiles}), " +
               $"Data: {_bytesDownloaded / 1024.0 / 1024.0:F2} MB, " +
               $"Elapsed: {elapsed:mm\\:ss}, " +
               $"Rate: {filesPerSec:F2} files/s, {mbPerSec:F2} MB/s, " +
               $"ETA: {eta:mm\\:ss} ({confidence}), " +
               $"Errors: {_errors}";
    }

    public IEnumerable<(string Folder, int Files, double MB)> GetFolderSummaries()
    {
        foreach (KeyValuePair<string, FolderStats> kvp in _folderStats)
            yield return (kvp.Key, kvp.Value.FileCount, kvp.Value.TotalBytes / 1024.0 / 1024.0);
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
