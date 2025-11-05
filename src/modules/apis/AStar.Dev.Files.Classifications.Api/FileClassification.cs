namespace AStar.Dev.Files.Classifications.Api;

/// <summary>
/// Represents a classification of files, containing details about name, search level, parent classification, and flags for celebrity status or search inclusion.
/// </summary>
public record FileClassification(Guid? Id, int? SearchLevel, Guid? ParentId, string? Name, bool? Celebrity, bool? IncludeInSearch);
