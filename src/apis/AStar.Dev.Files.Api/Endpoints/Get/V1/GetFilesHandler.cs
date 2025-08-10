using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public class GetFilesHandler : IGetFilesHandler
{
    /// <inheritdoc />
    public async Task<IResult> HandleAsync(GetFilesRequest files, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken)
    {
        if(files.SearchType is SearchType.DuplicateImages or SearchType.Duplicates)
        {
            return Results.BadRequest();
        }

        var searchType = Enum.Parse<SearchType>(files.SearchType.ToString());

        var sortOrder = (Infrastructure.FilesDb.Models.SortOrder) Enum.Parse<SortOrder>(files.SortOrder
                                                                                             .ToString());

        IList<GetFilesResponse> fileDetails = await filesContext.FileDetails
                                                                .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)

                                                                //.IncludeMarkedForDeletion(files.IncludeMarkedForDeletion)
                                                                .SelectFilesMatching(files.SearchText)

                                                                //.OrderAsRequested(sortOrder)
                                                                //.SelectByFileType(searchType)
                                                                .WhereLastViewedIsOlderThan(files.ExcludeViewedWithinDays, time)

                                                                //.SelectRequestedPage(files.CurrentPage, files.ItemsPerPage)
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
