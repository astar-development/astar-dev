using AStar.Dev.Infrastructure.FilesDb.Data;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public static class GetFiles
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
        await Task.CompletedTask;

        throw new NotImplementedException();
    }
}
