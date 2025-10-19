using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Mvc;

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
        var versionedApi = endpointRouteBuilder.NewVersionedApi("FileClassifications");

        var apiGroup = versionedApi
            .MapGroup("/api/file-classifications")
            .HasApiVersion(1.0);

        apiGroup.MapGet("/", async ([FromServices] FilesContext filesContext,
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
