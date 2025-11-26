using System.Diagnostics;
using AStar.Dev.OneDrive.Client.ViewModels;

namespace AStar.Dev.OneDrive.Client.Services;

public sealed class ProgressReporter(MainWindowViewModel vm, MetricsCollector metrics, int fileInterval = 5, int msInterval = 500)
{
    private readonly TimeSpan _timeInterval = TimeSpan.FromMilliseconds(msInterval);

    private int _sinceLastFiles;
    private long _lastTicks = Stopwatch.GetTimestamp();

    public void OnFileCompleted()
    {
        var nowTicks = Stopwatch.GetTimestamp();
        _sinceLastFiles++;

        var elapsed = new TimeSpan(nowTicks - _lastTicks);
        if(_sinceLastFiles >= fileInterval || elapsed >= _timeInterval)
        {
            vm.ReportProgress(metrics.GetGlobalSummary());
            _sinceLastFiles = 0;
            _lastTicks = nowTicks;
        }
    }

    public void Flush() => vm.ReportProgress(metrics.GetGlobalSummary());
}
