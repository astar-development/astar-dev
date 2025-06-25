using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AStar.Dev.Restful.Root.Document.TestEndpoints;

/// <summary>
///     The <see cref="RootEndpoint.Get" /> class used to retrieve the default root document.
/// </summary>
[ApiController]
[Route("/")]
[ApiVersion(1.0)]
public sealed class GetRoot
{
    /// <summary>
    ///     Gets something.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token</param>
    /// <returns>The root document</returns>
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointDescription("This endpoint gets something at the root level")]
    [HttpGet("{id}")]
    public ActionResult<int> GetRootDocument(int id, CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested
            ? (ActionResult<int>)new BadRequestResult()
            : (ActionResult<int>)new OkObjectResult(id);
}
