namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
public class ChannelBacklogTracker
{
    private int _count;

    /// <summary>
    /// </summary>
    public int Count => Volatile.Read(ref _count);

    /// <summary>
    /// </summary>
    public void Increment() => Interlocked.Increment(ref _count);

    /// <summary>
    /// </summary>
    public void Decrement() => Interlocked.Decrement(ref _count);
}
