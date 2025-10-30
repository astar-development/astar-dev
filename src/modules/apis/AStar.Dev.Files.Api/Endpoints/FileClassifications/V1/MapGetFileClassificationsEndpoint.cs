using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AStar.Dev.Files.Api.Endpoints.FileClassifications.V1;

/// <summary>
///     Maps the file classifications GET endpoint
/// </summary>
public static class MapGetFileClassificationsEndpoint
{
    /// <summary>
    ///     Maps the file classifications GET endpoint to the route builder
    /// </summary>
    /// <param name="endpointRouteBuilder">The endpoint route builder</param>
    public static void MapFileClassificationsGetEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        IVersionedEndpointRouteBuilder versionedApi = endpointRouteBuilder.NewVersionedApi("FileClassifications");

        RouteGroupBuilder apiGroup = versionedApi
            .MapGroup("/api/file-classifications")
            .HasApiVersion(1.0);

        _ = apiGroup.MapGet("/", async ([FromServices] FilesContext filesContext,
                    [FromServices] GetFileClassificationsHandler handler,
                    CancellationToken cancellationToken)
                => await handler.HandleAsync(filesContext, cancellationToken))
            .Produces<IReadOnlyCollection<GetFileClassificationsResponse>>()
            .Produces(401)
            .Produces(403)
            .WithName("GetFileClassifications")
            .WithTags("FileClassifications");
    }
}
