// using Ardalis.ApiEndpoints;
// using AStar.Dev.Infrastructure.FilesDb.Data;
// using AStar.Dev.Utilities;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace AStar.Dev.Files.Api.Endpoints;
//
// /// <summary>
// /// </summary>
// [Route("api/files")]
// public sealed class Update(FilesContext context, ILogger<Update> logger)
//     : EndpointBaseAsync.WithRequest<DirectoryChangeRequest>.WithActionResult
// {
//     /// <summary>
//     /// </summary>
//     [HttpPut("update-directory")]
//     [SwaggerOperation(
//                          Summary = "Updates the file directory",
//                          Description = "Updates the file directory from the specified old directory to the specified new directory.",
//                          OperationId = "Files_UpdateDirectory",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult> HandleAsync(DirectoryChangeRequest request,
//                                                          CancellationToken      cancellationToken = default)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//         request = UpdateOldDirectoryName(request);
//         request = UpdateNewDirectoryName(request);
//
//         var specifiedFile = await context.Files.FirstOrDefaultAsync(
//                                                                     file => file.DirectoryName == request.OldDirectoryName && file.FileName == request.FileName,
//                                                                     cancellationToken);
//
//         var error = string.Empty;
//
//         if(specifiedFile != null)
//         {
//             var newLocation = specifiedFile.DirectoryName.Replace(request.OldDirectoryName, request.NewDirectoryName);
//
//             try
//             {
//                 _ = Directory.CreateDirectory(request.NewDirectoryName);
//                 var newNameAndLocation = Path.Combine(newLocation, request.FileName);
//
//                 if(System.IO.File.Exists(specifiedFile.FullNameWithPath))
//                 {
//                     System.IO.File.Move(specifiedFile.FullNameWithPath, newNameAndLocation, true);
//                 }
//             }
//             catch(DirectoryNotFoundException ex)
//             {
//                 logger.LogError(ex,
//                                 "The specified directory could not be found. File: {FileName} in Directory: {OldDirectoryName} or target directory: {NewDirectoryName}",
//                                 request.FileName, request.NewDirectoryName, request.OldDirectoryName);
//
//                 error =
//                     $"The specified directory could not be found. File: {request.FileName} in Directory: {request.OldDirectoryName} or target directory: {request.NewDirectoryName}";
//             }
//             catch(FileNotFoundException ex)
//             {
//                 logger.LogError(ex,
//                                 "The specified file could not be found. File: {FileName} in Directory: {OldDirectoryName} or target directory: {NewDirectoryName}",
//                                 request.FileName, request.NewDirectoryName, request.OldDirectoryName);
//
//                 error = $"The specified file could not be found. File: {request.FileName} in Directory: {request.OldDirectoryName}";
//             }
//
//             var targetFile = await context.Files.FirstOrDefaultAsync(
//                                                                      file => file.DirectoryName == newLocation && file.FileName == request.FileName, cancellationToken);
//
//             if(targetFile != null)
//             {
//                 _ = context.Files.Remove(targetFile);
//             }
//
//             _ = context.Files.Remove(specifiedFile);
//             _ = await context.SaveChangesAsync(cancellationToken);
//
//             var fileDetail = await context.FileAccessDetails.FirstAsync(fileDetail => fileDetail.Id == specifiedFile.Id,
//                                                                         cancellationToken);
//
//             fileDetail.SoftDeletePending  = false;
//             fileDetail.DetailsLastUpdated = DateTime.UtcNow;
//             specifiedFile.DirectoryName   = request.NewDirectoryName;
//             _                             = await context.Files.AddAsync(specifiedFile, cancellationToken);
//             _                             = await context.SaveChangesAsync(cancellationToken);
//         }
//
//         logger.LogDebug(
//                         "File {FileName} was moved to the new {NewDirectory} directory from the {OldDirectory} directory",
//                         request.FileName, request.NewDirectoryName, request.OldDirectoryName);
//
//         return error.IsNotNullOrWhiteSpace()
//                    ? BadRequest(error)
//                    : NoContent();
//     }
//
//     private static DirectoryChangeRequest UpdateNewDirectoryName(DirectoryChangeRequest request)
//     {
//         if(request.NewDirectoryName.EndsWith('\\'))
//         {
//             request.NewDirectoryName = request.NewDirectoryName.Remove(request.NewDirectoryName.Length - 1);
//         }
//
//         if(request.NewDirectoryName.EndsWith('\\'))
//         {
//             request.NewDirectoryName = request.NewDirectoryName.Remove(request.NewDirectoryName.Length - 1);
//         }
//
//         return request;
//     }
//
//     private static DirectoryChangeRequest UpdateOldDirectoryName(DirectoryChangeRequest request)
//     {
//         if(request.OldDirectoryName.EndsWith('\\'))
//         {
//             request.OldDirectoryName = request.OldDirectoryName.Remove(request.OldDirectoryName.Length - 1);
//         }
//
//         if(request.OldDirectoryName.EndsWith('\\'))
//         {
//             request.OldDirectoryName = request.OldDirectoryName.Remove(request.OldDirectoryName.Length - 1);
//         }
//
//         return request;
//     }
// }


