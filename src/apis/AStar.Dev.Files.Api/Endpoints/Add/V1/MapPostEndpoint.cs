using System.Security.Claims;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Mvc;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
///     As the name suggests, this class contains the Map Files Endpoint method
/// </summary>
public static class MapPostEndpoint
{
    /// <summary>
    ///     As the name suggests, this method will map the Files Post endpoint specifically
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    public static void MapFilesPostEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var versionedApi = endpointRouteBuilder.NewVersionedApi(EndpointConstants.AddFilesGroupName);

        var apiGroup = versionedApi
                       .MapGroup(EndpointConstants.FilesEndpoint)
                       .HasApiVersion(1.0);

        _ = apiGroup.MapPost("/", async (AddFilesRequest files, [FromServices] FilesContext filesContext, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
                                  => await PostedFilesHandler.HandleAsync(files, filesContext, TimeProvider.System, claimsPrincipal.Identity?.Name ?? "Jay Barden", cancellationToken))
                .Produces<IReadOnlyCollection<AddFilesResponse>>(201)
                .Produces(401)
                .Produces(403);
    }
}
