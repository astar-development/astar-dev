using System.Collections.Concurrent;

namespace AStar.Dev.Database.Updater.FileKeywordProcessor;

public class ThroughputTracker
{
    private readonly ConcurrentQueue<DateTime> _events = new();

    public void RecordEvent() => _events.Enqueue(DateTime.UtcNow);

    public int CountEventsInWindow(TimeSpan window)
    {
        var cutoff = DateTime.UtcNow - window;

        while(_events.TryPeek(out var ts) && ts < cutoff)
        {
            _events.TryDequeue(out _);
        }

        return _events.Count;
    }
}
