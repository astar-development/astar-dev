namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
///     Represents a file classification
/// </summary>
public class FileClassification
{
    /// <summary>
    ///     Gets or sets the unique identifier for the file classification
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets or sets the name of the file classification
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     Gets or sets whether this classification should be included in search
    /// </summary>
    public required bool IncludeInSearch { get; init; }

    /// <summary>
    ///     Gets or sets whether this classification is marked as a celebrity
    /// </summary>
    public required bool Celebrity { get; init; }
}
