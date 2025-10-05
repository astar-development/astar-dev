using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <typeparam name="T"></typeparam>
public class TrackedChannel<T>
{
    private readonly Channel<T> _inner;
    private          int        _count;

    /// <summary>
    /// </summary>
    /// <param name="inner"></param>
    public TrackedChannel(Channel<T> inner)
    {
        _inner = inner;
        Writer = new(inner.Writer, this);
        Reader = new(inner.Reader, this);
    }

    /// <summary>
    /// </summary>
    public TrackingWriter Writer { get; }

    /// <summary>
    /// </summary>
    public TrackingReader Reader { get; }

    /// <summary>
    /// </summary>
    public int Count => Volatile.Read(ref _count);

    private void Increment() => Interlocked.Increment(ref _count);
    private void Decrement() => Interlocked.Decrement(ref _count);

    /// <summary>
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="parent"></param>
    public class TrackingWriter(ChannelWriter<T> inner, TrackedChannel<T> parent)
    {
        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryWrite(T item)
        {
            if(inner.TryWrite(item))
            {
                parent.Increment();

                return true;
            }

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
        {
            parent.Increment();

            return inner.WriteAsync(item, cancellationToken);
        }

        /// <summary>
        /// </summary>
        /// <param name="error"></param>
        public void Complete(Exception? error = null) => inner.Complete(error);
    }

    /// <summary>
    /// </summary>
    /// <param name="inner"></param>
    /// <param name="parent"></param>
    public class TrackingReader(ChannelReader<T> inner, TrackedChannel<T> parent)
    {
        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            var item = await inner.ReadAsync(cancellationToken);
            parent.Decrement();

            return item;
        }

        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<T> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach(var item in inner.ReadAllAsync(cancellationToken))
            {
                parent.Decrement();

                yield return item;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryRead(out T? item)
        {
            if(inner.TryRead(out item))
            {
                parent.Decrement();

                return true;
            }

            item = default;

            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default)
            => inner.WaitToReadAsync(cancellationToken);
    }
}
