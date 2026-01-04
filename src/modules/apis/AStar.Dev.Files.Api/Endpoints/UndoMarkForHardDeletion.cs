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
// public sealed class UndoMarkForHardDeletion(FilesContext context, ILogger<UndoMarkForHardDeletion> logger)
//     : EndpointBaseAsync.WithRequest<Request>.WithActionResult
// {
//     /// <summary>
//     /// </summary>
//     [HttpPut("undo-mark-for-hard-deletion")]
//     [SwaggerOperation(
//                          Summary = "Undo marking the specified file for hard deletion",
//                          Description =
//                              "Undo marking the specified file for hard deletion - the file will NOT be deleted, just UndoMarked for hard deletion, please run the separate delete method to actually delete the file.",
//                          OperationId = "Files_UndoMarkForHardDeletion",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult> HandleAsync(Request request, CancellationToken cancellationToken = default)
//     {
//         var specifiedFile = await context.FileAccessDetails.FirstOrDefaultAsync(file => file.Id == request.Id, cancellationToken);
//
//         if(specifiedFile != null)
//         {
//             specifiedFile.HardDeletePending = false;
//             _                               = await context.SaveChangesAsync(cancellationToken);
//
//             logger.LogDebug("File {FileName} mark for hard deletion has been undone", request);
//
//             return NoContent();
//         }
//
//         logger.LogDebug("File {FileName} could not be found - mark for hard deletion cannot be undone", request);
//
//         return NotFound();
//     }
// }


