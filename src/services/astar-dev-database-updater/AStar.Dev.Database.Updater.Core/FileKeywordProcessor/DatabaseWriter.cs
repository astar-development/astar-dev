using System.Threading.Channels;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="reader"></param>
/// <param name="filesContext"></param>
/// <param name="batchSize"></param>
/// <param name="logger"></param>
public class DatabaseWriter(ChannelReader<FileDetail> reader, FilesContext filesContext, int batchSize = 5_000, ILogger<DatabaseWriter> logger = null!)
{
    /// <summary>
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task ConsumeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("DatabaseWriter - Starting saving in the ConsumeAsync method");
        var buffer = new List<FileDetail>(batchSize);

        await foreach(var item in reader.ReadAllAsync(cancellationToken))
        {
            buffer.Add(item);

            if(buffer.Count < batchSize)
            {
                File.AppendAllText("logs/batch.log.txt", buffer.Count + Environment.NewLine);
                logger.LogInformation("DatabaseWriter - Buffer size is {Count}", buffer.Count);

                continue;
            }

            logger.LogInformation("DatabaseWriter - Adding {FileKeywordMatch} to the buffer", item);
            await SaveBatchAsync(buffer, cancellationToken);
            buffer.Clear();
        }

        logger.LogInformation("DatabaseWriter - Finished reading from the reader");

        if(buffer.Count > 0)
        {
            logger.LogInformation("DatabaseWriter - Saving {Count} remaining items", buffer.Count);
            await SaveBatchAsync(buffer, cancellationToken);
        }

        logger.LogInformation("DatabaseWriter - Finished saving in the ConsumeAsync method");
    }

    private async Task SaveBatchAsync(List<FileDetail> batch, CancellationToken cancellationToken)
    {
        await filesContext.Files.AddRangeAsync(batch, cancellationToken);
        await filesContext.SaveChangesAsync(cancellationToken);
    }
}
