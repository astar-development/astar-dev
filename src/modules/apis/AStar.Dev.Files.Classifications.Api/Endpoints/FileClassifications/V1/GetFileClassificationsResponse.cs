namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Response model for file classification data
/// </summary>
public record GetFileClassificationsResponse(Guid Id, string Name, bool IncludeInSearch, bool Celebrity, Guid? ParentId, string? Parent = null, int SearchLevel = 2);
