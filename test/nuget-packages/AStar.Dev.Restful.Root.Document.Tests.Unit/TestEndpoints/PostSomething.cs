using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AStar.Dev.Restful.Root.Document.TestEndpoints;

[ApiController]
[Route("/post")]
[ApiVersion(1.0)]
internal sealed class PostSomething
{
    /// <summary>
    ///     Creates something...
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token</param>
    /// <returns>Whether the item was deleted or not</returns>
    /// [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [EndpointDescription("Creates something description...")]
    [HttpPost("{id}")]
    public ActionResult<bool> CreateTask(CancellationToken cancellationToken) =>
        cancellationToken.IsCancellationRequested
            ? (ActionResult<bool>)new BadRequestResult()
            : (ActionResult<bool>)new OkObjectResult(true);
}
