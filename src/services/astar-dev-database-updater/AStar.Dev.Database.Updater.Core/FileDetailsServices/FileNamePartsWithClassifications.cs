using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Database.Updater.Core.FileDetailsServices;

/// <summary>
/// </summary>
public class FileNamePartsWithClassifications
{
    /// <summary>
    ///     Gets or sets the unique identifier for the <see cref="FileNamePart" /> entity.
    ///     This property serves as the primary key in the database to distinguish
    ///     each record of file name parts.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the name of the file classification.
    ///     This property represents the descriptive label for a specific classification
    ///     and is often used to identify or categorize files within the database.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets a value indicating whether the file classification is considered a "Celebrity."
    ///     This property is used to mark specific classifications with special significance.
    /// </summary>
    public bool Celebrity { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this classification should be included in search results.
    ///     This property determines if files associated with this classification are considered searchable.
    /// </summary>
    public bool IncludeInSearch { get; set; }

    /// <summary>
    ///     Gets or sets the collection of file name parts associated with the file classification.
    ///     This property represents the one-to-many relationship between a file classification
    ///     and its constituent parts that define or describe its naming structure.
    /// </summary>
    public List<FileNamePart> FileNameParts { get; set; } = [];
}
