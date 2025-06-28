using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AStar.Dev.Restful.Root.Document.TestEndpoints;

[ApiController]
[Route("/")]
[ApiVersion(1.0)]
public sealed class GetOptions
{
    /// <summary>
    ///     This endpoint wont be included in the output.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token</param>
    /// <returns>The root document</returns>
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointDescription("This endpoint wont be included in the output - description")]
    [HttpOptions("{id}")]
    public ActionResult<int> GetRootDocument(int id, CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested
            ? (ActionResult<int>)new BadRequestResult()
            : (ActionResult<int>)new OkObjectResult(id);
}
