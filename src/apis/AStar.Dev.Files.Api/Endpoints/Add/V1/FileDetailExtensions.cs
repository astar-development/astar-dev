using AStar.Dev.Infrastructure.FilesDb.Models;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
///     The <see cref="FileDetailExtensions"/> class contains extensions for the <see cref="FileDetail"/> class.
/// </summary>
public static class FileDetailExtensions
{
    /// <summary>
    ///     The ToAddFilesResponse will, as the name suggests, map the list of <see cref="FileDetail"/> to a list of <see cref="AddFilesResponse"/>
    /// </summary>
    /// <param name="fileDetails">The files to map to <see cref="AddFilesResponse"/></param>
    /// <returns>The <see cref="IReadOnlyCollection{AddFilesResponse}"/></returns>
    public static IReadOnlyCollection<AddFilesResponse> ToAddFilesResponse(this IReadOnlyCollection<FileDetail> fileDetails)
        => fileDetails
           .Select(file => new AddFilesResponse
                           {
                               FileName         = file.FileName,
                               DirectoryName    = file.DirectoryName,
                               Id               = file.Id,
                               FileCreated      = file.FileCreated,
                               ImageDetails     = new () { Width = file.ImageDetail?.Width, Height = file.ImageDetail?.Height },
                               FileHandle       = file.FileHandle,
                               FileSize         = file.FileSize,
                               FileLastModified = file.FileLastModified,
                               IsImage          = file.IsImage
                           })
           .ToList();

    /// <summary>
    ///     The ToFileDetailsList will, as the name suggests, map the list of <see cref="FileDetailToAdd"/> to a list of <see cref="FileDetail"/>
    /// </summary>
    /// <param name="fileDetails">The files to map to <see cref="FileDetail"/></param>
    /// <param name="time">An instance of the <see cref="TimeProvider"/></param>
    /// <param name="username">The name of the user performing the update</param>
    /// <returns>The <see cref="IReadOnlyCollection{FileDetail}"/></returns>
    public static IReadOnlyCollection<FileDetail> ToFileDetailsList(this IReadOnlyCollection<FileDetailToAdd> fileDetails, TimeProvider time, string username)
        => fileDetails.Select(fileDetailToAdd => new FileDetail
                                                 {
                                                     FileName         = fileDetailToAdd.FileName,
                                                     DirectoryName    = fileDetailToAdd.DirectoryName,
                                                     FileCreated      = fileDetailToAdd.FileCreated,
                                                     FileLastModified = fileDetailToAdd.FileLastModified,
                                                     FileSize         = fileDetailToAdd.FileSize,
                                                     FileHandle       = "???",
                                                     UpdatedBy        = username,
                                                     ImageDetail      = new(fileDetailToAdd.ImageDetails.Width, fileDetailToAdd.ImageDetails.Height),
                                                     IsImage          = true,
                                                     UpdatedOn        = time.GetUtcNow()
                                                 })
                      .ToList();

    /// <summary>
    ///     The ToEvents will, as the name suggests, map the list of <see cref="FileDetailToAdd" /> to a list of <see cref="Event" />
    /// </summary>
    /// <param name="fileDetails">The files to map to Events</param>
    /// <param name="time">An instance of the <see cref="TimeProvider" /></param>
    /// <param name="username">The name of the user performing the update</param>
    /// <returns>The <see cref="IReadOnlyCollection{Events}" /></returns>
    public static IReadOnlyCollection<Event> ToEvents(this IReadOnlyCollection<FileDetailToAdd> fileDetails, TimeProvider time, string username)
        => fileDetails.Select(fileDetailToAdd => new Event
                                                 {
                                                     FileName         = fileDetailToAdd.FileName,
                                                     DirectoryName    = fileDetailToAdd.DirectoryName,
                                                     FileCreated      = fileDetailToAdd.FileCreated,
                                                     FileLastModified = fileDetailToAdd.FileLastModified,
                                                     FileSize         = fileDetailToAdd.FileSize,
                                                     UpdatedBy        = username,
                                                     EventOccurredAt  = time.GetUtcNow(),
                                                     Type             = EventType.Add,
                                                     Handle           = "???",
                                                     Height           = fileDetailToAdd.ImageDetails.Height,
                                                     Width            = fileDetailToAdd.ImageDetails.Width
                                                 })
                      .ToList();
}
