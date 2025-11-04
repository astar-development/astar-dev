using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.ClassificationsServices;

/// <summary>
///     The <see cref="FileClassificationsBackgroundService" /> class contains the available service(s) available for the <see cref="ClassificationMapping" /> class
/// </summary>
/// <param name="serviceScopeFactory">The instance of <see cref="IServiceScopeFactory" /> to get the required services from</param>
public class FileClassificationsBackgroundService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private ILogger<FileClassificationsBackgroundService> _logger = null!;

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await AddNewMappingsToTheDatabase(stoppingToken);

    /// <summary>
    ///     The AddNewMappingsToTheDatabase method will process the mappings and add any new mappings to the context
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    public async Task<Result<bool, ErrorResponse>> AddNewMappingsToTheDatabase(CancellationToken stoppingToken)
    {
        using IServiceScope scope                   = serviceScopeFactory.CreateScope();
        ClassificationProcessor classificationProcessor = scope.ServiceProvider.GetRequiredService<ClassificationProcessor>();
        ClassificationsMapper mapper                  = scope.ServiceProvider.GetRequiredService<ClassificationsMapper>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<FileClassificationsBackgroundService>>();

        _ = await mapper.LoadClassificationMappings()
                    .MatchAsync(
                        classifications => classificationProcessor.ProcessAsync(classifications, stoppingToken),
                                error =>
                                {
                                    _logger.LogError("Error: {ErrorMessage}", error.Message);

                                    return true;
                                });

        _logger.LogDebug("Loaded any new mappings...");

        return new Result<bool, ErrorResponse>.Ok(true);
    }
}
