namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Response model for file classification data
/// </summary>
public record GetFileClassificationsResponse(Guid Id, string Name, bool IncludeInSearch, bool Celebrity, FileClassification? Parent = null, int SearchLevel = 2);
