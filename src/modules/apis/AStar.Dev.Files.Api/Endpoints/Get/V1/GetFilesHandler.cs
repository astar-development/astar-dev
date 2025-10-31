using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public class GetFilesHandler : IGetFilesHandler
{
    /// <inheritdoc />
    public async Task<IResult> HandleAsync(GetFilesRequest files, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken)
    {
        if(files.SearchType is SearchType.DuplicateImages or SearchType.Duplicates) return Results.BadRequest();

        IList<GetFilesResponse> fileDetails = await filesContext.Files
                                                                .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)
                                                                .SelectFilesMatching(files.SearchText)
                                                                .WhereLastViewedIsOlderThan(files.ExcludeViewedWithinDays, time)
                                                                .Select(fileDetail => fileDetail.ToGetFilesResponse())
                                                                .ToListAsync(cancellationToken);

        return Results.Ok(fileDetails);
    }
}

/// <summary>
/// </summary>
public interface IGetFilesHandler
{
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="filesContext"></param>
    /// <param name="time"></param>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult> HandleAsync(GetFilesRequest files, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken);
}
