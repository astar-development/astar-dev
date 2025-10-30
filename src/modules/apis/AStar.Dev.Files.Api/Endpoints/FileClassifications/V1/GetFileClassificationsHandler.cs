using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Handler for retrieving file classifications
/// </summary>
public class GetFileClassificationsHandler
{
    /// <summary>
    ///     Handles the request to get all file classifications
    /// </summary>
    /// <param name="filesContext">The database context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A collection of file classifications</returns>
    public async Task<IReadOnlyCollection<GetFileClassificationsResponse>> HandleAsync(
        FilesContext filesContext,
        CancellationToken cancellationToken)
    {
        List<GetFileClassificationsResponse> classifications = await filesContext.FileClassifications
            .OrderBy(fc => fc.Name)
            .Select(fc => new GetFileClassificationsResponse { Id = fc.Id, Name = fc.Name, IncludeInSearch = fc.IncludeInSearch, Celebrity = fc.Celebrity })
            .ToListAsync(cancellationToken);

        return classifications;
    }
}
