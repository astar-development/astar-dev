using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
/// </summary>
public static class PostedFiles // move to non-static and IoC?
{
    private const int MaxFilesToAdd = 100; // From config?

    /// <summary>
    /// </summary>
    /// <param name="files"></param>
    /// <param name="filesContext"></param>
    /// <param name="time"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IResult> Handle(AddFilesRequest files, FilesContext filesContext, TimeProvider time, CancellationToken cancellationToken)
    {
        if(files.FilesToAdd.Count > MaxFilesToAdd)
        {
            return Results.BadRequest($"Too many files supplied. Please try again with {MaxFilesToAdd} files or less.");
        }

        var fileDetailList = files.FilesToAdd.ToFileDetailsList(time);

        await filesContext.AddRangeAsync(fileDetailList, cancellationToken);
        await filesContext.SaveChangesAsync(cancellationToken);

        var responseList = fileDetailList.ToAddFilesResponse();

        return TypedResults.CreatedAtRoute(responseList, "GetFiles");
    }
}
