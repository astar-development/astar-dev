namespace AStar.Dev.Web.Models;

/// <summary>
///     Represents a classification of files, providing metadata about the type of files
///     and associated entities such as file details and file name parts.
/// </summary>
public class FileClassification
{
    // public FileClassification(Files.Classifications.Api.FileClassification fc)
    // {
    //     Id = fc.Id;
    //     Name = fc.Name;
    //     Celebrity = fc.Celebrity;
    //     IncludeInSearch = fc.IncludeInSearch;
    //     SearchLevel = fc.SearchLevel;
    //     ParentId = fc.ParentId;
    // }

    /// <summary>
    ///     Gets or sets the unique identifier for the file classification.
    ///     This property serves as the primary key for the <see cref="FileClassification" /> entity.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// </summary>
    public int? SearchLevel { get; set; }

    /// <summary>
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    ///     Gets or sets the name of the file classification.
    ///     This property represents the descriptive label for a specific classification
    ///     and is often used to identify or categorize files within the database.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the file classification is considered a "Celebrity."
    ///     This property is used to mark specific classifications with special significance.
    /// </summary>
    public bool? Celebrity { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this classification should be included in search results.
    ///     This property determines if files associated with this classification are considered searchable.
    /// </summary>
    public bool? IncludeInSearch { get; set; }
}
