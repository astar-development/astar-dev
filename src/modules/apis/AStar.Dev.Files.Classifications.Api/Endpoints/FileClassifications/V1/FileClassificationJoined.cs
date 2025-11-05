using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
/// Provides an extension method to join file classifications with their parent classifications.
/// </summary>
public static class FileClassificationJoined
{
    /// <summary>
    /// Joins file classifications with their associated parent classifications using a left join approach.
    /// Each file classification is paired with its parent classification, if available.
    /// </summary>
    /// <param name="query">The database queryable set of file classifications.</param>
    /// <returns>An <see cref="IQueryable{FileClassifications}"/> containing the joined file classification and parent classification data.</returns>
    public static IQueryable<FileClassifications> JoinFileClassificationsToParents(this DbSet<AStar.Dev.Infrastructure.FilesDb.Models.FileClassification> query)
        => query.AsNoTracking()
            .GroupJoin(
                query,
                fc => fc.ParentId,
                p => p.Id,
                (fc, parents) => new { fc, parents }
            )
            .SelectMany(x => x.parents.DefaultIfEmpty(), (x, p) => new FileClassifications( x.fc, p ));
}
