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
// public sealed class MarkForHardDeletion(FilesContext context, ILogger<MarkForHardDeletion> logger)
//     : EndpointBaseAsync.WithRequest<Request>.WithActionResult
// {
//     /// <summary>
//     /// </summary>
//     [HttpPut("mark-for-hard-deletion")]
//     [SwaggerOperation(
//                          Summary = "Mark the specified file for hard deletion",
//                          Description =
//                              "Mark the specified file for hard deletion - the file will NOT be deleted, just marked for hard deletion, please run the separate delete method to actually delete the file.",
//                          OperationId = "Files_MarkForHardDeletion",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult> HandleAsync(Request request, CancellationToken cancellationToken = default)
//     {
//         var specifiedFile = await context.FileAccessDetails.FirstOrDefaultAsync(file => file.Id == request.Id, cancellationToken);
//
//         if(specifiedFile != null)
//         {
//             specifiedFile.HardDeletePending = true;
//             _                               = await context.SaveChangesAsync(cancellationToken);
//
//             logger.LogDebug("File {FileName} marked for hard deletion", request);
//
//             return NoContent();
//         }
//
//         logger.LogDebug("File {FileName} could not be found - mark for hard deletion cannot be performed", request);
//
//         return NotFound();
//     }
// }


