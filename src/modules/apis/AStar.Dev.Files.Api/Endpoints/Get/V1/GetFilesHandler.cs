using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public class GetFilesHandler : IGetFilesHandler
{
    /// <inheritdoc />
    public async Task<IResult> HandleAsync(GetFilesRequest fileClassifications, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken)
    {
        if(fileClassifications.SearchType is SearchType.DuplicateImages or SearchType.Duplicates) return Results.BadRequest();

        IList<GetFilesResponse> fileDetails = await filesContext.Files
            .WhereDirectoryNameMatches(fileClassifications.SearchFolder, fileClassifications.Recursive)
            .SelectFilesMatching(fileClassifications.SearchText)
            .WhereLastViewedIsOlderThan(fileClassifications.ExcludeViewedWithinDays, time)
                                                                .Select(fileDetail => fileDetail.ToGetFilesResponse())
                                                                .ToListAsync(cancellationToken);

        return Results.Ok(fileDetails);
    }
}
