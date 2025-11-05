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
        // Normalize paging: ensure [1..50] page size and page index >= 1
        var pageSize = fileClassifications.ItemsPerPage <= 0
            ? 10
            : (fileClassifications.ItemsPerPage > 50 ? 50 : fileClassifications.ItemsPerPage);
        var pageIndex = fileClassifications.CurrentPage <= 0 ? 1 : fileClassifications.CurrentPage;
        var skip = (pageIndex - 1) * pageSize;

        // Left join to parent (self-referential), null-safe; project directly to response
        IQueryable<GetFileClassificationsResponse> query =
            from fc in filesContext.FileClassifications.AsNoTracking()
            join p in filesContext.FileClassifications.AsNoTracking()
                on fc.ParentId equals p.Id into parentGroup
            from p in parentGroup.DefaultIfEmpty()
            orderby fc.Name, fc.Id
            select new GetFileClassificationsResponse(
                fc.Id,
                fc.Name,
                fc.IncludeInSearch,
                fc.Celebrity,
                fc.ParentId,
                p != null ? p.Name : null,
                fc.SearchLevel
            );

        List<GetFileClassificationsResponse> results = await query
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return results;
    }
}
