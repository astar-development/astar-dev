using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
/// </summary>
public static class FileDetailExtensions
{
    /// <summary>
    /// </summary>
    /// <param name="fileDetails"></param>
    /// <returns></returns>
    public static object ToAddFilesResponse(this IList<FileDetail> fileDetails)
        => fileDetails
           .Select(file => new AddFilesResponse
                           {
                               FileName         = file.FileName,
                               DirectoryName    = file.DirectoryName,
                               Id               = file.Id,
                               FileCreated      = file.FileCreated,
                               ImageDetails     = new () { Width = file.ImageDetails.Width, Height = file.ImageDetails.Height },
                               FileHandle       = file.FileHandle,
                               FileSize         = file.FileSize,
                               FileLastModified = file.FileLastModified,
                               IsImage          = file.IsImage
                           })
           .ToList();

    /// <summary>
    ///     1
    /// </summary>
    /// <param name="fileDetails"></param>
    /// <param name="time"></param>
    /// <returns></returns>
    public static IList<FileDetail> ToFileDetailsList(this IReadOnlyCollection<FileDetailToAdd> fileDetails, TimeProvider time)
        => fileDetails.Select(fileDetailToAdd => new FileDetail
                                                 {
                                                     FileName         = fileDetailToAdd.FileName,
                                                     DirectoryName    = fileDetailToAdd.DirectoryName,
                                                     FileCreated      = fileDetailToAdd.FileCreated,
                                                     FileLastModified = fileDetailToAdd.FileLastModified,
                                                     FileSize         = fileDetailToAdd.FileSize,
                                                     FileHandle       = "???",
                                                     ModifiedBy       = "TBC",
                                                     ImageDetails     = new() { Width  = fileDetailToAdd.ImageDetails.Width, Height = fileDetailToAdd.ImageDetails.Height },
                                                     IsImage          = true,
                                                     DetailsModified  = time.GetUtcNow()
                                                 })
                      .ToList();
}
