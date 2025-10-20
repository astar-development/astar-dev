using AStar.Dev.Database.Updater.Core.Models;

namespace AStar.Dev.Database.Updater.Core.ClassificationsServices;

/// <summary>
///     The <see cref="ClassificationMappingExtensions" /> containing the ClassificationMapping extensions
/// </summary>
public static class ClassificationMappingExtensions
{
    /// <summary>
    ///     Adds classification mappings to the database context, creating missing entries.
    /// </summary>
    /// <param name="mappings">The classification mappings to process.</param>
    /// <param name="classificationProcessor">The ClassificationProcessor to use.</param>
    /// <param name="stoppingToken"></param>
    /// <returns><c>true</c> if successful, otherwise <c>false</c></returns>
    public static async Task<bool> AddFileClassificationsAsync(this IEnumerable<ClassificationMapping> mappings, ClassificationProcessor classificationProcessor,
                                                               CancellationToken                       stoppingToken = default)
    {
        _ = await classificationProcessor.ProcessAsync(mappings, stoppingToken);

        return true; // ToDo -  Result rather than raw boolean
    }
}
