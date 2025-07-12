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
        await filesContext.FileDetails
                          .WhereDirectoryNameMatches(files.SearchFolder, files.Recursive)
                          .ToListAsync(cancellationToken);

        throw new NotImplementedException();
    }
}
