using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
///     Service responsible for processing keywords in files within a specified directory.
///     It extends the <see cref="BackgroundService" /> class, enabling it to run as a
///     hosted background task. The service periodically scans files for processing based
///     on configurations provided at runtime.
/// </summary>
/// <remarks>
///     This service uses dependency injection to resolve required services such as
///     <see cref="Microsoft.Extensions.Logging.ILogger" />, <c>FileScanner</c>, and <c>FileListService</c>.
///     It coordinates the scanning and processing of files, handling scenarios including
///     error logging for failed operations.
///     Dependencies must be registered in the DI container before using this service.
/// </remarks>
public class FileKeywordProcessorBackgroundService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private ILogger<FileKeywordProcessorBackgroundService> _logger = null!;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        (DatabaseUpdaterConfiguration? databaseUpdaterConfiguration, IFilesProcessor? fileScanner, IFileListService? fileListService) = GetRequiredServices();

        if(databaseUpdaterConfiguration.RunNewFilesService)
        {
            _ = await fileListService.Get(databaseUpdaterConfiguration.RootDirectory, stoppingToken)
                .MatchAsync(
                    fileList => fileScanner.ProcessAsync(fileList, stoppingToken),
                    error =>
                    {
                        _logger.LogError("Error in FileKeywordProcessorService: {Error}", error.Message);

                        return Task.CompletedTask;
                    });
        }
    }

    private (DatabaseUpdaterConfiguration config, IFilesProcessor fileScanner, IFileListService fileListService) GetRequiredServices()
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        DatabaseUpdaterConfiguration config = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseUpdaterConfiguration>>().Value;
        IFilesProcessor fileScanner = scope.ServiceProvider.GetRequiredService<IFilesProcessor>();
        IFileListService fileListService = scope.ServiceProvider.GetRequiredService<IFileListService>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<FileKeywordProcessorBackgroundService>>();

        return (config, fileScanner, fileListService);
    }
}
