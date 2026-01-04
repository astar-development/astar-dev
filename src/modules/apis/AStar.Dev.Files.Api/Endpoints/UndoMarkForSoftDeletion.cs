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
// public sealed class UndoMarkForSoftDeletion(FilesContext context, ILogger<MarkForSoftDeletion> logger)
//     : EndpointBaseAsync.WithRequest<Request>.WithActionResult
// {
//     /// <summary>
//     /// </summary>
//     [HttpPut("undo-mark-for-soft-deletion")]
//     [SwaggerOperation(
//                          Summary = "Undo marking the specified file for soft deletion",
//                          Description = "Undo marking the specified file for soft deletion.",
//                          OperationId = "Files_UndoMarkForSoftDeletion",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult> HandleAsync(Request request, CancellationToken cancellationToken = default)
//     {
//         var specifiedFile = await context.FileAccessDetails.FirstOrDefaultAsync(file => file.Id == request.Id, cancellationToken);
//
//         if(specifiedFile != null)
//         {
//             specifiedFile.SoftDeletePending = false;
//             _                               = await context.SaveChangesAsync(cancellationToken);
//
//             logger.LogDebug("File {FileName} mark for soft deletion has been undone", specifiedFile);
//
//             return NoContent();
//         }
//
//         logger.LogDebug("File {FileName} could not be found - undo mark for soft deletion cannot be performed",
//                         specifiedFile);
//
//         return NotFound();
//     }
// }


