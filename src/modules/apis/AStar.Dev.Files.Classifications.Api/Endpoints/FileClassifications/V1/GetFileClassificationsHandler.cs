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
        if(fileClassifications.ItemsPerPage > 50) fileClassifications = fileClassifications with { ItemsPerPage = 50 };
        
        List<GetFileClassificationsResponse> classifications = await filesContext.FileClassifications
            .OrderBy(fc => fc.Name)
            .Skip(fileClassifications.CurrentPage - 1).Take(fileClassifications.ItemsPerPage)
            .Select(fc => new GetFileClassificationsResponse(fc.Id, fc.Name, fc.IncludeInSearch, fc.Celebrity))
            .ToListAsync(cancellationToken);

        return classifications;
    }
}
