using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Serilog;

namespace AStar.Dev.Database.Updater.Core.ClassificationsServices;

/// <summary>
///     Coordinates the classification processing workflow: loading, creating, and updating.
/// </summary>
public class ClassificationProcessor(ClassificationRepository repository, ClassificationBuilder builder, ILogger logger)
{
    /// <summary>
    ///     Processes classification mappings and updates the database context accordingly.
    /// </summary>
    /// <param name="mappings">The input classification mappings.</param>
    /// <param name="stoppingToken"></param>
    /// <returns>True if processing completes successfully.</returns>
    public async Task<bool> ProcessAsync(IEnumerable<ClassificationMapping> mappings, CancellationToken stoppingToken)
    {
        var mappingsList  = mappings.ToList();
        var distinctNames = mappingsList.Select(m => m.DatabaseMapping).ToHashSet();
        var existing      = repository.GetExistingClassifications(distinctNames);

        var newClassifications = CreateMissingClassifications(mappingsList, distinctNames, existing);

        if(newClassifications.Count > 0)
        {
            repository.AddClassifications(newClassifications);
            logger.Debug("Added {Count} new classifications", newClassifications.Count);
        }

        AddMissingParts(mappingsList, existing);

        await repository.SaveChangesAsync(stoppingToken);

        return true;
    }

    private List<FileClassification> CreateMissingClassifications(List<ClassificationMapping> mappings, HashSet<string> names, Dictionary<string, FileClassification> existing)
    {
        var newClassifications = new List<FileClassification>();

        foreach(var name in names)
        {
            if(existing.ContainsKey(name))
            {
                continue;
            }

            var source = mappings.FirstOrDefault(m => m.DatabaseMapping == name);

            if(source == null)
            {
                continue;
            }

            var classification = builder.CreateClassification(name, source);
            newClassifications.Add(classification);
            existing[name] = classification;
        }

        return newClassifications;
    }

    private void AddMissingParts(List<ClassificationMapping> mappings, Dictionary<string, FileClassification> existing)
    {
        foreach(var group in mappings.GroupBy(m => m.DatabaseMapping))
        {
            if(!existing.TryGetValue(group.Key, out var classification))
            {
                continue;
            }

            var existingTexts = classification.FileNameParts
                                              .Select(p => p.Text)
                                              .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newParts = builder.CreateMissingParts(group, existingTexts);

            if(newParts.Count != 0)
            {
                classification.FileNameParts.AddRange(newParts);
            }
        }
    }
}
