using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
/// </summary>
public static class GetFilesResponseExtensions
{
    // /// <summary>
    // /// </summary>
    // /// <param name="detail"></param>
    // /// <returns></returns>
    // /// <exception cref="NotImplementedException"></exception>
    // public static GetFilesResponse ToGetFilesResponse(this DuplicateDetail detail)
    //     => new() { DirectoryName = detail.DirectoryName, FileName = detail.FileName, FileHandle = detail.FileHandle };

    /// <summary>
    /// </summary>
    /// <param name="detail"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static GetFilesResponse ToGetFilesResponse(this FileDetail detail)
        => new() { DirectoryName = detail.DirectoryName.Value, FileName = detail.FileName.Value, FileHandle = detail.FileHandle };
}
