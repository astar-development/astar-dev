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
        
        var results = await filesContext.FileClassifications
            .GroupJoin(filesContext.FileClassifications,
                d => d.Id,
                e => e.ParentId,
                (dep, emps) => new { dep, emps })
            .SelectMany(
                g=> g.emps.DefaultIfEmpty(),
                (a, e) => new 
                {
                    FileClassification = a.dep,
                    Parent = e
                }).ToListAsync(cancellationToken);
        
        var returnResults = results.Select(f=> new GetFileClassificationsResponse(f.FileClassification.Id, f.FileClassification.Name, f.FileClassification.IncludeInSearch, f.FileClassification.Celebrity, new FileClassification(f.Parent?.Id, f.Parent?.SearchLevel, f.Parent?.ParentId, f.Parent?.Name, f.FileClassification.Celebrity, f.FileClassification.IncludeInSearch), f.FileClassification.SearchLevel))
            .Skip(fileClassifications.CurrentPage - 1)
            .Take(fileClassifications.ItemsPerPage)
            .ToList();
        return returnResults;
    }
}
