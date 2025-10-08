using System.Threading.Channels;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="reader"></param>
/// <param name="serviceScopeFactory"></param>
/// <param name="batchSize"></param>
/// <param name="logger"></param>
public class DatabaseWriter(ChannelReader<FileDetail> reader, IServiceScopeFactory serviceScopeFactory, int batchSize = 200, ILogger<DatabaseWriter> logger = null!)
{
    /// <summary>
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task ConsumeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("DatabaseWriter - Starting saving in the ConsumeAsync method");
        var buffer = new List<FileDetail>(batchSize);
        File.Delete("logs/buffer.log.txt");
        File.Delete("logs/test.log.txt");
        await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);

        while(FileKeywordProcessorService.FileKeywordProcessorRunning)
        {
            await File.AppendAllTextAsync("logs/buffer.log.txt", "DatabaseWriter - Buffer Count: " + buffer.Count + " remaining items " + Environment.NewLine, cancellationToken);
            await ReadFromChannelAsync(cancellationToken, buffer);
        }

        logger.LogInformation("DatabaseWriter - Finished reading from the reader");

        if(buffer.Count > 0)
        {
            await File.AppendAllTextAsync("logs/buffer.log.txt", "DatabaseWriter - Final save of: " + buffer.Count + " remaining items " + Environment.NewLine, cancellationToken);
            logger.LogInformation("DatabaseWriter - Final save of: {Count} remaining items", buffer.Count);
            await SaveBatchAsync(buffer, cancellationToken);
        }

        logger.LogInformation("DatabaseWriter - Finished saving in the ConsumeAsync method");
    }

    private async Task ReadFromChannelAsync(CancellationToken cancellationToken, List<FileDetail> buffer)
    {
        await foreach(var item in reader.ReadAllAsync(cancellationToken))
        {
            try
            {
                buffer.Add(item);

                if(buffer.Count < batchSize)
                {
                    continue;
                }

                logger.LogInformation("DatabaseWriter - Adding {FileKeywordMatch} to the buffer", item);

                await File.AppendAllTextAsync("logs/buffer.log.txt", $"{DateTimeOffset.UtcNow}{Environment.NewLine}{buffer.Count} (Saving to Database) - {item.FileName}{Environment.NewLine}",
                                              cancellationToken);

                await SaveBatchAsync(buffer, cancellationToken);
                buffer.Clear();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);

                await File.AppendAllTextAsync("logs/test.log.txt", $"{DateTimeOffset.UtcNow}{Environment.NewLine}Error: {e}{Environment.NewLine}", cancellationToken);
                await SaveBatchAsync(buffer, cancellationToken);
                buffer.Clear();
            }
        }
    }

    private async Task SaveBatchAsync(List<FileDetail> batch, CancellationToken cancellationToken)
    {
        await File.AppendAllTextAsync("logs/buffer.log.txt", $"{DateTimeOffset.UtcNow}{Environment.NewLine}{batch.Count} (Saving to Database) - {batch[0].FileName}{Environment.NewLine}",
                                      cancellationToken);

        using var scope        = serviceScopeFactory.CreateScope();
        var       filesContext = scope.ServiceProvider.GetRequiredService<FilesContext>();
        await filesContext.Files.AddRangeAsync(batch, cancellationToken);
        await filesContext.SaveChangesAsync(cancellationToken);
    }
}
