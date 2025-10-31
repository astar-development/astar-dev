using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
/// </summary>
public static class PostedFilesHandler // move to non-static and IoC?
{
    private const int MaxFilesToAdd = 100; // From config?

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="filesContext"></param>
    /// <param name="time"></param>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> HandleAsync(AddFilesRequest files, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken)
    {
        if(files.FilesToAdd.Count > MaxFilesToAdd) return Results.BadRequest($"Too many files supplied. Please try again with {MaxFilesToAdd} files or less.");

        IReadOnlyCollection<FileDetail> fileDetailList = files.FilesToAdd.ToFileDetailsList(time, username);

        _ = files.FilesToAdd.ToEvents(time, username);

        await filesContext.Files.AddRangeAsync(fileDetailList, cancellationToken);

        _ = await filesContext.SaveChangesAsync(cancellationToken);

        IReadOnlyCollection<AddFilesResponse> responseList = fileDetailList.ToAddFilesResponse();

        // Need a "Get this list" version of the new Get Files
        return TypedResults.CreatedAtRoute(responseList, "Get Files");
    }
}
