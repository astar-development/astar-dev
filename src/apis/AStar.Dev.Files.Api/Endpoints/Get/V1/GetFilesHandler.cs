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
        IList<GetFilesResponse> fileDetails;

        if(files.SearchType is SearchType.DuplicateImages or SearchType.Duplicates)
        {
            // fileDetails = await filesContext.DuplicateDetails
            //                                 .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)
            //                                 .ExcludeViewed(files.ExcludeViewedWithinDays, time)
            //                                 .IncludeMarkedForDeletion(files.IncludeMarkedForDeletion)
            //                                 .SelectFilesMatching(files.SearchText)
            //                                 .OrderAsRequested(files.SortOrder)
            //                                 .SelectByFileType(files.SearchType)
            //                                 .SelectRequestedPage(files.CurrentPage, files.ItemsPerPage)
            //                                 .Select(fileDetail => fileDetail.ToGetFilesResponse())
            //                                 .ToListAsync(cancellationToken);
            return Results.BadRequest();
        }
        else
        {
            fileDetails = await filesContext.FileDetails
                                            .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)
                                            .ExcludeViewed(files.ExcludeViewedWithinDays, time)
                                            .IncludeMarkedForDeletion(files.IncludeMarkedForDeletion)
                                            .SelectFilesMatching(files.SearchText)
                                            .OrderAsRequested(files.SortOrder)
                                            .SelectByFileType(files.SearchType)
                                            .SelectRequestedPage(files.CurrentPage, files.ItemsPerPage)
                                            .Select(fileDetail => fileDetail.ToGetFilesResponse())
                                            .ToListAsync(cancellationToken);
        }

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
