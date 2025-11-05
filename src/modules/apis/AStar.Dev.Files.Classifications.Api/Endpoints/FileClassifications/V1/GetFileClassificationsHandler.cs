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
        var pagingParams = PagingParams.CreateValid(fileClassifications);
        
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
            .Skip(pagingParams.SkipValue)
            .Take(pagingParams.PageSize)
            .Select(x => new GetFileClassificationsResponse(
                x.fc.Id,
                x.fc.Name,
                x.fc.IncludeInSearch,
                x.fc.Celebrity,
                x.fc.ParentId,
                x.p != null ? x.p.Name : null,
                x.fc.SearchLevel
            ))
            .ToListAsync(cancellationToken);
    }
}
