using System.Globalization;
using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Functional.Extensions;
using CsvHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AStar.Dev.Database.Updater.Core.ClassificationsServices;

/// <summary>
///     The <see cref="ClassificationsMapper" /> that handles adding any new file > database classifications
/// </summary>
/// <param name="config"></param>
/// <param name="logger"></param>
/// <remarks>Before we can progress to testing this, we need to change the ClassificationsMapper to be more testable</remarks>
public class ClassificationsMapper(IOptions<DatabaseUpdaterConfiguration> config, ILogger<ClassificationsMapper> logger)
{
    /// <summary>
    ///     The LoadClassificationMappings does exactly what its name says
    /// </summary>
    /// <returns>An instance of <see cref="Result{TSuccess,TFailure}" /> to denote the success / failure of the load</returns>
    public Result<IEnumerable<ClassificationMapping>, ErrorResponse> LoadClassificationMappings()
        => Try.Run<IEnumerable<ClassificationMapping>>(() =>
                                                       {
                                                           logger.LogDebug("Loading mappings from the CSV...");
                                                           using var reader = new StreamReader(config.Value.MappingsFilePath);
                                                           using var csv    = new CsvReader(reader, CultureInfo.InvariantCulture);

                                                           var mappings = csv.GetRecords<ClassificationMapping>().ToList();
                                                           logger.LogDebug("Loaded mappings from the CSV...");

                                                           return mappings;
                                                       })
                                                       .MapFailure<IEnumerable<ClassificationMapping>, Exception, ErrorResponse>(ex => new ErrorResponse($"Failed to read mappings: {ex.Message}"));
}
