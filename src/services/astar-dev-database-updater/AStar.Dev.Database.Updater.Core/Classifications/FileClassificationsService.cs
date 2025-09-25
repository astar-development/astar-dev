using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Functional.Extensions;
using Microsoft.Extensions.Logging;

namespace AStar.Dev.Database.Updater.Core.Classifications;

/// <summary>
///     The <see cref="FileClassificationsService" /> class contains the available service(s) available for the <see cref="ClassificationMapping" /> class
/// </summary>
/// <param name="classificationProcessor">The instance of <see cref="ClassificationProcessor" /> to use in the available service(s)</param>
/// <param name="mapper">An instance of the <see cref="ClassificationMapping" /> that handles getting the classification mappings</param>
/// <param name="logger">A logger that can be used to, well, log information</param>
public class FileClassificationsService(ClassificationProcessor classificationProcessor, ClassificationsMapper mapper, ILogger<FileClassificationsService> logger)
{
    /// <summary>
    ///     The AddNewMappingsToTheDatabase method will process the mappings and add any new mappings to the context
    /// </summary>
    /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
    public async Task<Result<bool, ErrorResponse>> AddNewMappingsToTheDatabase(CancellationToken stoppingToken)
    {
        await mapper.LoadClassificationMappings()
                    .MatchAsync(classifications => classifications.AddFileClassificationsAsync(classificationProcessor, stoppingToken), LogError);

        logger.LogDebug("Loaded any new mappings...");

        return new Result<bool, ErrorResponse>.Ok(true);
    }

    private bool LogError(ErrorResponse error)
    {
        logger.LogError("Error: {ErrorMessage}", error.Message);

        return true;
    }
}
