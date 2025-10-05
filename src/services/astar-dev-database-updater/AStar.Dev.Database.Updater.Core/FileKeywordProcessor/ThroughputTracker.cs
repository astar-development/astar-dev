using System.Collections.Concurrent;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
///     Tracks throughput events over time using a TimeProvider.
/// </summary>
public class ThroughputTracker(TimeProvider timeProvider)
{
    private readonly ConcurrentQueue<DateTimeOffset> _events       = new();
    private readonly TimeProvider                    _timeProvider = timeProvider;

    /// <summary>
    ///     Records a new event at the current time.
    /// </summary>
    public void RecordEvent() => _events.Enqueue(_timeProvider.GetUtcNow());

    /// <summary>
    ///     Returns the number of events within the given time window.
    /// </summary>
    public int CountEventsInWindow(TimeSpan window)
    {
        var cutoff = _timeProvider.GetUtcNow() - window;

        while(_events.TryPeek(out var ts) && ts < cutoff)
        {
            _events.TryDequeue(out _);
        }

        return _events.Count;
    }
}
