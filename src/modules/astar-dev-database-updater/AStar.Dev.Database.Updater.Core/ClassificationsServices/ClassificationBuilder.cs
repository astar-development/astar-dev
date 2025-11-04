using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.ClassificationsServices;

/// <summary>
///     Responsible for constructing FileClassification and FileNamePart entities.
/// </summary>
public class ClassificationBuilder
{
    /// <summary>
    ///     Creates a new FileClassification from a mapping source.
    /// </summary>
    /// <param name="name">The classification name.</param>
    /// <param name="source">The source mapping containing metadata.</param>
    /// <returns>A new FileClassification instance.</returns>
    public FileClassification CreateClassification(string name, ClassificationMapping source)
        => new()
        {
            Name = name,
            Celebrity = source.Celebrity,
            IncludeInSearch = source.Searchable,
            FileNameParts = [],
            SearchLevel = source.SearchLevel
        };

    /// <summary>
    ///     Creates a list of missing FileNameParts based on mappings and existing texts.
    /// </summary>
    /// <param name="mappings">Mappings to extract file name parts from.</param>
    /// <param name="existingTexts">Set of already existing file name texts.</param>
    /// <returns>List of new FileNamePart entities.</returns>
    public List<FileNamePart> CreateMissingParts(IEnumerable<ClassificationMapping> mappings, HashSet<string> existingTexts)
        => mappings
           .Select(m => m.FileNameContains)
           .Where(text => !existingTexts.Contains(text))
           .Distinct(StringComparer.OrdinalIgnoreCase)
           .Select(text => new FileNamePart { Text = text })
           .ToList();
}
