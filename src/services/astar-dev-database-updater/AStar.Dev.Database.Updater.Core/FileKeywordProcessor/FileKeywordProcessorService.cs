using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core.FileKeywordProcessor;

/// <summary>
/// </summary>
/// <param name="serviceScopeFactory"></param>
public class FileKeywordProcessorService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var       enumerationOptions = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
        using var scope              = serviceScopeFactory.CreateScope();
        var       fileSystem         = scope.ServiceProvider.GetRequiredService<IFileSystem>();
        var       config             = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseUpdaterConfiguration>>();
        var       fileScanner        = scope.ServiceProvider.GetRequiredService<FileScanner>();
        var       writer             = scope.ServiceProvider.GetRequiredService<DatabaseWriter>();
        var       filePaths          = fileSystem.Directory.GetFiles(config.Value.RootDirectory, "*", enumerationOptions);

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FileKeywordProcessorService>>();
        logger.LogInformation("Starting scanning files - FileKeywordProcessorService");
        var producer = fileScanner.ScanFilesAsync(filePaths, stoppingToken);
        var consumer = writer.ConsumeAsync(stoppingToken);

        logger.LogInformation("Starting scanning files - FileKeywordProcessorService - Task.WhenAll");
        await Task.WhenAll(producer, consumer);
        logger.LogInformation("Starting scanning files - FileKeywordProcessorService - Task.WhenAll - Done");
    }
}
