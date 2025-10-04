using System.Threading.Channels;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Api.FileKeywordProcessor;

public class DatabaseWriter(ChannelReader<FileKeywordMatch> reader, FilesContext filesContext, int batchSize = 5000, ILogger<DatabaseWriter> logger = null!)
{
    public async Task ConsumeAsync(CancellationToken cancellationToken = default)
    {
        var buffer = new List<FileKeywordMatch>(batchSize);

        await foreach(var item in reader.ReadAllAsync(cancellationToken))
        {
            logger.LogInformation("Adding {FileKeywordMatch} to the buffer", item);
            buffer.Add(item);

            if(buffer.Count < batchSize)
            {
                continue;
            }

            await SaveBatchAsync(buffer, cancellationToken);
            buffer.Clear();
        }

        if(buffer.Count > 0)
        {
            logger.LogInformation("Saving {Count} remaining items", buffer.Count);
            await SaveBatchAsync(buffer, cancellationToken);
        }

        logger.LogInformation("Finished saving");
    }

    private async Task SaveBatchAsync(List<FileKeywordMatch> batch, CancellationToken cancellationToken)
    {
        await filesContext.FileKeywordMatches.AddRangeAsync(batch, cancellationToken);
        await filesContext.SaveChangesAsync(cancellationToken);
    }
}
