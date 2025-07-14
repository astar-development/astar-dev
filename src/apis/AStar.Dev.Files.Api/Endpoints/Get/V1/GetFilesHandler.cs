using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public static class GetFilesHandler
{
    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="filesContext"></param>
    /// <param name="time"></param>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleAsync(GetFilesRequest files, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken)
    {
        IList<GetFilesResponse> fileDetails;

        if(files.SearchType is SearchType.DuplicateImages or SearchType.Duplicates)
        {
            fileDetails = await filesContext.DuplicateDetails
                                            .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)
                                            .ExcludeViewed(files.ExcludeViewedWithinDays)
                                            .IncludeMarkedForDeletion(files.IncludeMarkedForDeletion)
                                            .SelectFilesMatching(files.SearchText)
                                            .OrderAsRequested(files.SortOrder)
                                            .SelectByFileType(files.SearchType)
                                            .SelectRequestedPage(files.CurrentPage, files.ItemsPerPage)
                                            .Select(fileDetail => fileDetail.ToGetFilesResponse())
                                            .ToListAsync(cancellationToken);
        }
        else
        {
            fileDetails = await filesContext.FileDetails
                                            .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)
                                            .ExcludeViewed(files.ExcludeViewedWithinDays)
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
