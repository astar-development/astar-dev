using System.IO.Abstractions;
using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// </summary>
    public static bool FileKeywordProcessorRunning { get; private set; }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        FileKeywordProcessorRunning = true;

        if(FileClassificationsService.ClassificationsLoading)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
        }

        var       enumerationOptions       = new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, ReturnSpecialDirectories = false };
        using var scope                    = serviceScopeFactory.CreateScope();
        var       fileSystem               = scope.ServiceProvider.GetRequiredService<IFileSystem>();
        var       config                   = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseUpdaterConfiguration>>();
        var       fileScanner              = scope.ServiceProvider.GetRequiredService<FileScanner>();
        var       writer                   = scope.ServiceProvider.GetRequiredService<DatabaseWriter>();
        var       filesContext             = scope.ServiceProvider.GetRequiredService<FilesContext>();
        var       filePaths                = fileSystem.Directory.GetFiles(config.Value.RootDirectory, "*", enumerationOptions);
        var       filesAlreadyInTheContext = await filesContext.Files.AsNoTracking().Select(f => f.FullNameWithPath).ToListAsync(stoppingToken);
        var       filesNotInTheDatabase    = filePaths.Except(filesAlreadyInTheContext).ToArray();

        var filesToProcessCount = filesNotInTheDatabase.Length;
        var filesToProcess      = new List<FileDetail>(filesToProcessCount);
        filesToProcess.AddRange(filesNotInTheDatabase.Select(file => fileSystem.FileInfo.New(file)).Select(fileInfo => new FileDetail(fileInfo) { Id = new() { Value = Guid.CreateVersion7() } }));

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<FileKeywordProcessorService>>();
        logger.LogInformation("Starting scanning files - FileKeywordProcessorService");
        var producer = fileScanner.ScanFilesAsync(filesToProcess, stoppingToken);
        var consumer = writer.ConsumeAsync(stoppingToken);

        logger.LogInformation("Starting scanning files - FileKeywordProcessorService - Task.WhenAll");
        await Task.WhenAll(producer, consumer);
        logger.LogInformation("Starting scanning files - FileKeywordProcessorService - Task.WhenAll - Done");
        FileKeywordProcessorRunning = false;
    }
}
