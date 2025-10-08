using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
public static class ChannelReadBatch
{
    /// <summary>
    ///     Consumes the items in the channel in batches. Each batch contains all
    ///     the items that are immediately available, up to a specified maximum number.
    /// </summary>
    public static IAsyncEnumerable<T[]> ReadBatchImmediateAsync<T>(
        this ChannelReader<T> source, int maxSize = -1)
    {
        ArgumentNullException.ThrowIfNull(source);

        if(maxSize == -1)
        {
            maxSize = Array.MaxLength;
        }

        if(maxSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxSize));
        }

        return Implementation();

        async IAsyncEnumerable<T[]> Implementation(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while(await source.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                List<T> buffer = new();

                while(buffer.Count < maxSize && source.TryRead(out var item))
                {
                    buffer.Add(item);
                }

                if(buffer.Count > 0)
                {
                    yield return buffer.ToArray();
                }
            }
        }
    }
}
