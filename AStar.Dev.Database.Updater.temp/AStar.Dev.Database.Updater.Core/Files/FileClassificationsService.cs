// using AStar.Dev.Database.Updater.Core.Models;
// using AStar.Dev.Functional.Extensions;
// using AStar.Dev.Infrastructure.FilesDb.Data;
// using Microsoft.Extensions.Logging;
//
// namespace AStar.Dev.Database.Updater.Core.Files;
//
// /// <summary>
// ///     The <see cref="FileClassificationsService" /> class contains the available service(s) available for the <see cref="ClassificationMapping" /> class
// /// </summary>
// /// <param name="filesContext">The instance of <see cref="FilesContext" /> to use in the available service(s)</param>
// /// <param name="mapper">An instance of the <see cref="ClassificationMapping" /> that handles obtaining the classification mappings</param>
// /// <param name="logger">A logger that can be used to, well, log information</param>
// public class FileClassificationsService(FilesContext filesContext, ClassificationsMapper mapper, ILogger<FileClassificationsService> logger)
// {
//     /// <summary>
//     ///     The AddNewMappingsToTheDatabase method will process the mappings and add any new mappings to the context
//     /// </summary>
//     /// <param name="stoppingToken">A cancellation token to optionally cancel the operation</param>
//     public async Task AddNewMappingsToTheDatabase(CancellationToken stoppingToken)
//     {
//         mapper.LoadClassificationMappings()
//               .Match(classifications => classifications.AddFileClassifications(filesContext), LogError);
//
//         await filesContext.SaveChangesAsync(stoppingToken);
//         logger.LogDebug("Loaded any new mappings...");
//     }
//
//     private bool LogError(Error error)
//     {
//         logger.LogError("Error: {ErrorMessage}", error.Message);
//
//         return true;
//     }
// }


