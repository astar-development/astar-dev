namespace AStar.Dev.Files.Api.Client.SDK.Models;

/// <summary>
/// </summary>
/// <param name="FileSize"></param>
/// <param name="Duplicates"></param>
public record DuplicatesList(FileSizeDetail FileSize, Duplicates[] Duplicates);
