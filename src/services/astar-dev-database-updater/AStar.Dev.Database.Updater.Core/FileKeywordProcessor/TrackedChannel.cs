using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// A channel wrapper that tracks backlog count while delegating to an inner channel.
/// Ensures items are non-null via a generic non-null constraint.
/// </summary>
/// <typeparam name="T">The non-nullable item type.</typeparam>
public sealed class TrackedChannel<T> where T : notnull
{
    private readonly Channel<T> _inner;
    private          int        _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrackedChannel{T}"/> class.
    /// </summary>
    /// <param name="inner">The inner channel to wrap.</param>
    public TrackedChannel(Channel<T> inner)
    {
        _inner = inner;
        Writer = new TrackingWriter(_inner.Writer, this);
        Reader = new TrackingReader(_inner.Reader, this);
    }

    /// <summary>
    /// Gets the tracked writer.
    /// </summary>
    public ChannelWriter<T> Writer { get; }

    /// <summary>
    /// Gets the tracked reader.
    /// </summary>
    public ChannelReader<T> Reader { get; }

    /// <summary>
    /// Gets the current backlog count.
    /// </summary>
    public int Count => Volatile.Read(ref _count);

    private void Increment() => Interlocked.Increment(ref _count);

    private void Decrement() => Interlocked.Decrement(ref _count);

    /// <summary>
    /// A writer that increments backlog count when items are written.
    /// Disallows null writes by contract.
    /// </summary>
    private sealed class TrackingWriter : ChannelWriter<T>
    {
        private readonly ChannelWriter<T>  _inner;
        private readonly TrackedChannel<T> _parent;

        public TrackingWriter(ChannelWriter<T> inner, TrackedChannel<T> parent)
        {
            _inner  = inner;
            _parent = parent;
        }

        /// <inheritdoc />
        public override bool TryWrite(T item)
        {
            // Runtime guard (belt and braces)
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if(_inner.TryWrite(item))
            {
                _parent.Increment();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override async ValueTask WriteAsync(T item, CancellationToken cancellationToken = default)
        {
            if(item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            await _inner.WriteAsync(item, cancellationToken);
            _parent.Increment();
        }

        /// <inheritdoc />
        public override ValueTask<bool> WaitToWriteAsync(CancellationToken cancellationToken = default) => _inner.WaitToWriteAsync(cancellationToken);
    }

    /// <summary>
    /// A reader that decrements backlog count when items are read.
    /// </summary>
    private sealed class TrackingReader : ChannelReader<T>
    {
        private readonly ChannelReader<T>  _inner;
        private readonly TrackedChannel<T> _parent;

        public TrackingReader(ChannelReader<T> inner, TrackedChannel<T> parent)
        {
            _inner  = inner;
            _parent = parent;
        }

        /// <inheritdoc />
        public override async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            var item = await _inner.ReadAsync(cancellationToken);

            // With T : notnull, item should never be null when successfully read.
            Debug.Assert(item is not null);

            _parent.Decrement();
            return item;
        }

        /// <inheritdoc />
        public override async IAsyncEnumerable<T> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach(var item in _inner.ReadAllAsync(cancellationToken))
            {
                Debug.Assert(item is not null);
                _parent.Decrement();
                yield return item;
            }
        }

        /// <inheritdoc />
        public override bool TryRead([MaybeNullWhen(false)] out T item)
        {
            if(_inner.TryRead(out item))
            {
                Debug.Assert(item is not null);
                _parent.Decrement();

                return true;
            }

            item = default!;

            return false;
        }

        /// <inheritdoc />
        public override ValueTask<bool> WaitToReadAsync(CancellationToken cancellationToken = default) => _inner.WaitToReadAsync(cancellationToken);

        /// <inheritdoc />
        public override bool TryPeek([MaybeNullWhen(false)] out T item)
        {
            if(_inner.TryPeek(out item))
            {
                Debug.Assert(item is not null);

                return true;
            }

            item = default!;

            return false;
        }
    }
}

