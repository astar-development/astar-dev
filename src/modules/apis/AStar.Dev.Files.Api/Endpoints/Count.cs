// using Ardalis.ApiEndpoints;
// using AStar.Dev.Files.Api.Config;
// using AStar.Dev.Infrastructure.FilesDb.Data;
// using AStar.Dev.Utilities;
// using Microsoft.AspNetCore.Mvc;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace AStar.Dev.Files.Api.Endpoints;
//
// /// <summary>
// /// </summary>
// [Route("api/files")]
// public sealed class Count(FilesContext context, ILogger<Count> logger)
//     : EndpointBaseAsync.WithRequest<CountSearchParameters>.WithActionResult<int>
// {
//     /// <summary>
//     /// </summary>
//     [HttpGet("count")]
//     [Produces("application/vnd.astar.file-count+json", "application/json")]
//     [ProducesResponseType(StatusCodes.Status200OK)]
//     [SwaggerOperation(
//                          Summary = "GetById the count of files",
//                          Description = "GetById the count of files matching the criteria",
//                          OperationId = "Files_Count",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult<int>> HandleAsync([FromQuery] CountSearchParameters request,
//                                                               CancellationToken                 cancellationToken = default)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//
//         if(request.SearchFolder.IsNullOrWhiteSpace())
//         {
//             return BadRequest(new { error = "A Search folder must be specified." });
//         }
//
//         if(request.SearchType == SearchType.Duplicates)
//         {
//             return BadRequest(
//                               "Duplicate searches are not supported by this endpoint, please call the duplicate-specific endpoint.");
//         }
//
//         var matchingFilesCount = context.Files
//                                         .GetMatchingFiles(request.SearchFolder,       request.Recursive,                request.SearchType.ToString(),
//                                                           request.IncludeSoftDeleted, request.IncludeMarkedForDeletion, request.ExcludeViewed, cancellationToken)
//                                         .Count();
//
//         logger.LogDebug("File Count: {FileCount}", matchingFilesCount);
//         await Task.Delay(1, cancellationToken);
//
//         return Ok(matchingFilesCount);
//     }
// }


