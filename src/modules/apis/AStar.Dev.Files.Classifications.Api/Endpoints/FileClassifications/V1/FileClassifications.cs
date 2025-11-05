namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
/// Represents the aggregation of a file classification and its optional parent file classification.
/// This record is utilized to handle hierarchical relationships between file classifications,
/// where a file classification can have a parent classification for organizational purposes.
/// </summary>
/// <param name="FileClassification">
/// The primary file classification data, representing a specific classification instance.
/// </param>
/// <param name="Parent">
/// The optional parent classification, indicating its hierarchical relationship to the primary
/// file classification. A null value signifies no parent classification.
/// </param>
public record FileClassifications(Infrastructure.FilesDb.Models.FileClassification FileClassification, Infrastructure.FilesDb.Models.FileClassification? Parent);
