using System.Security.Claims;
using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace AStar.Dev.Files.Api.Endpoints.Get.V1;

/// <summary>
///     As the name suggests, this class contains the Map Files Get Endpoint method
/// </summary>
public static class MapGetEndpoint
{
    /// <summary>
    ///     As the name suggests, this method will map the Files Get endpoint specifically
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    public static void MapFilesGetEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        IVersionedEndpointRouteBuilder versionedApi = endpointRouteBuilder.NewVersionedApi(EndpointConstants.GetFilesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                       .MapGroup(EndpointConstants.FilesEndpoint)
                       .HasApiVersion(1.0);

        _ = apiGroup.MapGet("/", async ([AsParameters] GetFilesRequest getFilesRequest, [FromServices] FilesContext filesContext, [FromServices] ClaimsPrincipal claimsPrincipal,
                    [FromServices] GetFilesHandler getFilesHandler, CancellationToken cancellationToken)
                => await getFilesHandler.HandleAsync(getFilesRequest, filesContext, TimeProvider.System, claimsPrincipal.Identity?.Name ?? "Jay Barden", cancellationToken))
                .Produces<IReadOnlyCollection<GetFilesResponse>>()
                .Produces(401)
                .Produces(403);
    }
}
