using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Classifications.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Handler for retrieving file classifications
/// </summary>
public class GetFileClassificationsHandler
{
    /// <summary>
    ///     Handles the request to get all file classifications
    /// </summary>
    /// <param name="fileClassifications"></param>
    /// <param name="filesContext">The database context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of file classifications</returns>
    public async Task<IReadOnlyCollection<GetFileClassificationsResponse>> HandleAsync(
        GetFileClassificationRequest fileClassifications, FilesContext filesContext,
        CancellationToken cancellationToken)
    {
        var pageSize = fileClassifications.ItemsPerPage <= 0
            ? 10
            : (fileClassifications.ItemsPerPage > 50 ? 50 : fileClassifications.ItemsPerPage);
        var pageIndex = fileClassifications.CurrentPage <= 0 ? 1 : fileClassifications.CurrentPage;
        var skip = (pageIndex - 1) * pageSize;

        return await filesContext.FileClassifications
            .AsNoTracking()
            .GroupJoin(
                filesContext.FileClassifications.AsNoTracking(),
                fc => fc.ParentId,
                p => p.Id,
                (fc, parents) => new { fc, parents }
            )
            .SelectMany(x => x.parents.DefaultIfEmpty(), (x, p) => new { x.fc, p })
            .OrderBy(x => x.fc.Name)
            .ThenBy(x => x.fc.Id)
            .Select(x => new GetFileClassificationsResponse(
                x.fc.Id,
                x.fc.Name,
                x.fc.IncludeInSearch,
                x.fc.Celebrity,
                x.fc.ParentId,
                x.p != null ? x.p.Name : null,
                x.fc.SearchLevel
            ))
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}
