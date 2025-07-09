using System.ComponentModel.DataAnnotations.Schema;

namespace AStar.Dev.Infrastructure.FilesDb.Models;

/// <summary>
/// </summary>
[ComplexType]
public record ImageDetail(int? Width, int? Height);
