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
// public sealed class FileAccessDetail(FilesContext context, ILogger<MarkForHardDeletion> logger)
//     : EndpointBaseAsync.WithRequest<int>.WithActionResult<FileAccessDetailDto>
// {
//     /// <summary>
//     /// </summary>
//     [HttpGet("access-detail")]
//     [SwaggerOperation(
//                          Summary = "Gets the key details for the file",
//                          Description = "Gets the key details for the file such as size and, if an image, width and height.",
//                          OperationId = "File_Detail",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult<FileAccessDetailDto>> HandleAsync(int              request,
//                                                                               CancellationToken cancellationToken = default)
//     {
//         var file = await context.Files.Include(file => file.FileAccessDetail)
//                                 .SingleOrDefaultAsync(file => file.Id == request, cancellationToken);
//
//         if(file != null)
//         {
//             return Ok(new FileAccessDetailDto(file.FileAccessDetail));
//         }
//
//         logger.LogDebug("File Access Details for FileId: {FileId} could not be found", request);
//
//         return NotFound();
//     }
// }


