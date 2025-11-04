using System.Globalization;
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
        TextInfo textInfo = new CultureInfo("en-GB").TextInfo;
        var mappingsList  = mappings.ToList();
        var distinctNames = mappingsList.Select(m => textInfo.ToTitleCase(m.DatabaseMapping)).ToHashSet();
        Dictionary<string, FileClassification> existing      = repository.GetExistingClassifications(distinctNames);

        List<FileClassification> newClassifications = CreateMissingClassifications(mappingsList, distinctNames, existing, textInfo);

        if(newClassifications.Count > 0)
        {
            repository.AddClassifications(newClassifications);
            logger.Debug("Added {Count} new classifications", newClassifications.Count);
        }

        AddMissingParts(mappingsList, existing, textInfo);

        await repository.SaveChangesAsync(stoppingToken);

        List<FileClassification> updatedClassifications = repository.GetExistingClassifications();
        var withParent = mappingsList.Where(m => !string.IsNullOrEmpty(m.ParentText)).ToList();

        if(withParent.Count == 0) return true;

        foreach(ClassificationMapping mapping in withParent)
        {
            try
            {
                FileClassification? parent = updatedClassifications.SingleOrDefault(c => c.Name == mapping.ParentText);
                if(parent == null) continue;
                FileClassification? child = updatedClassifications.SingleOrDefault(c => c.Name == mapping.DatabaseMapping && c.SearchLevel == mapping.SearchLevel);
                if(child == null) continue;
                child.ParentId = parent.Id;
            }
            catch(Exception e)
            {
                logger.Error(e, "Error processing classification mapping {Mapping}", mapping);
            }
        }

        await repository.SaveChangesAsync(stoppingToken);
        
        return true;
    }

    private List<FileClassification> CreateMissingClassifications(List<ClassificationMapping> mappings, HashSet<string> names, Dictionary<string, FileClassification> existing, TextInfo textInfo)
    {
        var newClassifications = new List<FileClassification>();

        foreach(var name in names)
        {
            if(existing.ContainsKey(name)) continue;

            ClassificationMapping? source = mappings.FirstOrDefault(m => textInfo.ToTitleCase(m.DatabaseMapping) == name);

            if(source == null) continue;

            FileClassification classification = builder.CreateClassification(name, source);
            newClassifications.Add(classification);
            existing[name] = classification;
        }

        return newClassifications;
    }

    private void AddMissingParts(List<ClassificationMapping> mappings, Dictionary<string, FileClassification> existing, TextInfo textInfo)
    {
        foreach(IGrouping<string, ClassificationMapping> group in mappings.GroupBy(m => m.DatabaseMapping))
        {
            if(!existing.TryGetValue(textInfo.ToTitleCase(group.Key), out FileClassification? classification)) continue;

            var existingTexts = classification.FileNameParts
                                              .Select(p => p.Text)
                                              .ToHashSet(StringComparer.OrdinalIgnoreCase);

            List<FileNamePart> newParts = builder.CreateMissingParts(group, existingTexts);

            if(newParts.Count != 0) classification.FileNameParts.AddRange(newParts);
        }
    }
}
