using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.Classifications;

/// <summary>
///     The <see cref="FileClassificationsService" /> class contains the available service(s) available for the <see cref="ClassificationMapping" /> class
/// </summary>
/// <param name="serviceScopeFactory">The instance of <see cref="IServiceScopeFactory" /> to obtain the required services from</param>
public class FileClassificationsService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private ILogger<FileClassificationsService> _logger = null!;

    /// <summary>
    /// </summary>
    public static bool ClassificationsLoading { get; private set; }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) => await AddNewMappingsToTheDatabase(stoppingToken);

    /// <summary>
    ///     The AddNewMappingsToTheDatabase method will process the mappings and add any new mappings to the context
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    public async Task<Result<bool, ErrorResponse>> AddNewMappingsToTheDatabase(CancellationToken stoppingToken)
    {
        using var scope                   = serviceScopeFactory.CreateScope();
        var       classificationProcessor = scope.ServiceProvider.GetRequiredService<ClassificationProcessor>();
        var       mapper                  = scope.ServiceProvider.GetRequiredService<ClassificationsMapper>();
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<FileClassificationsService>>();

        ClassificationsLoading = true;

        await mapper.LoadClassificationMappings()
                    .MatchAsync(classifications => classifications.AddFileClassificationsAsync(classificationProcessor, stoppingToken), LogError);

        ClassificationsLoading = false;
        _logger.LogDebug("Loaded any new mappings...");

        return new Result<bool, ErrorResponse>.Ok(true);
    }

    private bool LogError(ErrorResponse error)
    {
        _logger.LogError("Error: {ErrorMessage}", error.Message);

        return true;
    }
}
