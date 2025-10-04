namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public class ChannelBacklogTracker
{
    private int _count;
    public  int Count => Volatile.Read(ref _count);

    public void Increment() => Interlocked.Increment(ref _count);
    public void Decrement() => Interlocked.Decrement(ref _count);
}
