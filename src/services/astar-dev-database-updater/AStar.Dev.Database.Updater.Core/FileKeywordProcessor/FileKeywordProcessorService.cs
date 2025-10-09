using AStar.Dev.Database.Updater.Core.Classifications;
using AStar.Dev.Database.Updater.Core.Files;
using AStar.Dev.Functional.Extensions;
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
    private ILogger<FileKeywordProcessorService> _logger = null!;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if(FileClassificationsService.ClassificationsLoading)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(50), stoppingToken);
        }

        using var scope           = serviceScopeFactory.CreateScope();
        var       config          = scope.ServiceProvider.GetRequiredService<IOptions<DatabaseUpdaterConfiguration>>();
        var       fileScanner     = scope.ServiceProvider.GetRequiredService<FileScanner>();
        var       fileListService = scope.ServiceProvider.GetRequiredService<FileListService>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<FileKeywordProcessorService>>();

        var filesToProcess = fileListService.Get(config.Value.RootDirectory, stoppingToken);

        await filesToProcess.MatchAsync(f => fileScanner.ScanFilesAsync(f, stoppingToken),
                                        error => Task.FromResult(LogError(error)));
    }

    private Result<bool, ErrorResponse> LogError(ErrorResponse error)
    {
        _logger.LogError("Error in FileKeywordProcessorService: {Error}", error.Message);

        return new Result<bool, ErrorResponse>.Error(error);
    }
}
