namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Response model for file classification data
/// </summary>
public class GetFileClassificationsResponse
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
