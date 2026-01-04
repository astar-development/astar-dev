// using Ardalis.ApiEndpoints;
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
// public sealed class MarkForMoving(FilesContext context, ILogger<MarkForMoving> logger)
//     : EndpointBaseAsync.WithRequest<Request>.WithActionResult
// {
//     /// <summary>
//     /// </summary>
//     [HttpPut("mark-for-moving")]
//     [SwaggerOperation(
//                          Summary = "Mark the specified file for moving later",
//                          Description =
//                              "Mark the specified file for moving - the file will NOT be moved, just marked for moving. Please use the applicable page in the portal to actually perform the move.",
//                          OperationId = "Files_MarkForMoving",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult> HandleAsync(Request request, CancellationToken cancellationToken = default)
//     {
//         var specifiedFile = await context.FileAccessDetails.FirstOrDefaultAsync(file => file.Id == request.Id, cancellationToken);
//
//         if(specifiedFile != null)
//         {
//             specifiedFile.MoveRequired = true;
//             _                          = await context.SaveChangesAsync(cancellationToken);
//
//             logger.LogDebug("File {FileName} marked for moving", request);
//
//             return NoContent();
//         }
//
//         logger.LogDebug("File {FileName} not found - mark for moving cannot be performed", request);
//
//         return NotFound();
//     }
// }


