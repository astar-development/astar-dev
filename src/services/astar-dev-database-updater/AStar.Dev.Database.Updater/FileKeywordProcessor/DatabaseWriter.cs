using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading.Channels;

namespace AStar.Dev.Database.Updater.FileKeywordProcessor;

public class DatabaseWriter
{
    private readonly ActivitySource    _activitySource;
    private readonly Counter<long>     _batchesWritten;
    private readonly Histogram<double> _batchLatency;
    private readonly int               _batchSize;
    private readonly Meter             _meter;

    public DatabaseWriter(int batchSize = 5000)
    {
        _batchSize = batchSize;

        _meter          = new("DatabaseWriter");
        _batchesWritten = _meter.CreateCounter<long>("batches_written");
        _batchLatency   = _meter.CreateHistogram<double>("batch_latency_ms");

        _activitySource = new("DatabaseWriter");
    }

    public async Task ConsumeAsync(ChannelReader<FileKeywordMatch> reader,
                                   CancellationToken               cancellationToken = default)
    {
        var buffer = new List<FileKeywordMatch>(_batchSize);

        await foreach(var item in reader.ReadAllAsync(cancellationToken))
        {
            buffer.Add(item);

            if(buffer.Count >= _batchSize)
            {
                await SaveBatchAsync(buffer, cancellationToken);
                buffer.Clear();
            }
        }

        if(buffer.Count > 0)
        {
            await SaveBatchAsync(buffer, cancellationToken);
        }
    }

    private async Task SaveBatchAsync(List<FileKeywordMatch> batch, CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity("SaveBatch", ActivityKind.Client);
        var       sw       = Stopwatch.StartNew();

        using var db = new AppDbContext();
        await db.FileKeywordMatches.AddRangeAsync(batch, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        sw.Stop();
        _batchesWritten.Add(1);
        _batchLatency.Record(sw.Elapsed.TotalMilliseconds);
    }
}
