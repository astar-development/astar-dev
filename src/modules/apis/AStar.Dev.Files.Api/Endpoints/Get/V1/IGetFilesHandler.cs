using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Http;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public interface IGetFilesHandler
{
    /// <summary>
    /// </summary>
    /// <param name="fileClassifications"></param>
    /// <param name="filesContext"></param>
    /// <param name="time"></param>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IResult> HandleAsync(GetFilesRequest fileClassifications, FilesContext filesContext, TimeProvider time, string username, CancellationToken cancellationToken);
}
