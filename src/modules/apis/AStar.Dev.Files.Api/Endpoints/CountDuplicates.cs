// using Ardalis.ApiEndpoints;
// using AStar.Dev.Files.Api.Config;
// using AStar.Dev.Infrastructure.FilesDb;
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
// public sealed class CountDuplicates(FilesContext context, ILogger<CountDuplicates> logger)
//     : EndpointBaseAsync.WithRequest<CountDuplicatesSearchParameters>.WithActionResult<int>
// {
//     /// <summary>
//     /// </summary>
//     [HttpGet("count-duplicates")]
//     [SwaggerOperation(
//                          Summary = "GetById duplicate files count",
//                          Description = "GetById the count of duplicate files matching the criteria",
//                          OperationId = "Duplicates_Count",
//                          Tags = ["Files"]),
//     ]
//     public override async Task<ActionResult<int>> HandleAsync([FromQuery] CountDuplicatesSearchParameters request,
//                                                               CancellationToken                           cancellationToken = default)
//     {
//         ArgumentNullException.ThrowIfNull(request);
//
//         if(request.SearchFolder.IsNullOrWhiteSpace())
//         {
//             return BadRequest("A Search folder must be specified.");
//         }
//
//         if(request.SearchType != SearchType.Duplicates)
//         {
//             return BadRequest(
//                               "Only Duplicate Group counts are supported by this endpoint, please call the non-duplicate-specific endpoint.");
//         }
//
//         var matchingFiles = context.Files
//                                    .GetMatchingFiles(request.SearchFolder,       request.Recursive,                request.SearchType.ToString(),
//                                                      request.IncludeSoftDeleted, request.IncludeMarkedForDeletion, request.ExcludeViewed, cancellationToken)
//                                    .GetDuplicatesCount(cancellationToken);
//
//         logger.LogDebug("Duplicate File Count: {FileCount}", matchingFiles);
//         await Task.Delay(1, cancellationToken);
//
//         return Ok(matchingFiles);
//     }
// }


