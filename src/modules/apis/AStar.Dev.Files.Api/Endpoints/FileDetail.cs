// using Ardalis.ApiEndpoints;
// using AStar.Dev.Files.Api.Models;
// using AStar.Dev.Infrastructure.FilesDb.Data;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace AStar.Dev.Files.Api.Endpoints;
//
// /// <summary>
// /// </summary>
// [Route("api/files")]
// public sealed class FileDetail(FilesContext context, ILogger<MarkForHardDeletion> logger)
//     : EndpointBaseAsync.WithRequest<int>.WithActionResult<FileInfoDto>
// {
//     /// <summary>
//     /// </summary>
//     [HttpGet("detail")]
//     [SwaggerOperation(
//                          Summary = "Gets the key details for the file",
//                          Description = "Gets the key details for the file such as size and, if an image, width and height.",
//                          OperationId = "File_Detail",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult<FileInfoDto>> HandleAsync(int              request,
//                                                                       CancellationToken cancellationToken = default)
//     {
//         var specifiedFile = await context.Files.FirstOrDefaultAsync(file => file.Id == request, cancellationToken);
//
//         if(specifiedFile != null)
//         {
//             var fileAccessDetail = await context.FileAccessDetails.SingleAsync(file => file.Id == request, cancellationToken);
//             fileAccessDetail.LastViewed = DateTime.UtcNow;
//             _                           = await context.SaveChangesAsync(cancellationToken);
//
//             return Ok(new FileInfoDto(specifiedFile));
//         }
//
//         logger.LogDebug("File {FileId} could not be found", request);
//
//         return NotFound();
//     }
// }


