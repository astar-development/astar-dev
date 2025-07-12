using System.Security.Claims;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Mvc;

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
        var versionedApi = endpointRouteBuilder.NewVersionedApi(EndpointConstants.GetFilesGroupName);

        var apiGroup = versionedApi
                       .MapGroup(EndpointConstants.FilesEndpoint)
                       .HasApiVersion(1.0);

        apiGroup.MapGet("/", async ([AsParameters] GetFilesRequest files, [FromServices] FilesContext filesContext, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
                                 => await GetFilesHandler.HandleAsync(files, filesContext, TimeProvider.System, claimsPrincipal.Identity?.Name ?? "Jay Barden", cancellationToken))
                .Produces<IReadOnlyCollection<GetFilesResponse>>()
                .Produces(401)
                .Produces(403);
    }
}
