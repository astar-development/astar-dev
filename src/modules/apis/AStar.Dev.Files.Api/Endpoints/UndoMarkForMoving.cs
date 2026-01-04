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
// public sealed class UndoMarkForMoving(FilesContext context, ILogger<UndoMarkForMoving> logger)
//     : EndpointBaseAsync.WithRequest<Request>.WithActionResult
// {
//     /// <summary>
//     /// </summary>
//     [HttpPut("undo-mark-for-moving")]
//     [SwaggerOperation(
//                          Summary = "Undo marking the specified file for moving later",
//                          Description =
//                              "Undo marking the specified file for moving - the file will NOT be moved, just marked for moving. Please use the applicable page in the portal to actually perform the move.",
//                          OperationId = "Files_UndoMarkForMoving",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult> HandleAsync(Request request, CancellationToken cancellationToken = default)
//     {
//         var specifiedFile = await context.FileAccessDetails.FirstOrDefaultAsync(file => file.Id == request.Id, cancellationToken);
//
//         if(specifiedFile != null)
//         {
//             specifiedFile.MoveRequired = false;
//             _                          = await context.SaveChangesAsync(cancellationToken);
//
//             logger.LogDebug("File {FileName} mark for moving has been undonw", request);
//
//             return NoContent();
//         }
//
//         logger.LogDebug("File {FileName} could not be found - undo mark for moving cannot be done", request);
//
//         return NotFound();
//     }
// }


