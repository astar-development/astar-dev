using System.Security.Claims;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.AspNetCore.Mvc;

namespace AStar.Dev.Files.Api.Endpoints.Add.V1;

/// <summary>
///     As the name suggests, this class contains the Map Files Endpoint method(s)
///     Currently, it only supports the Post, and this may remain as, directly adding Get etc. would, IMO, violate SRP
///     However, I may use this as the entry point into a set of other classes, which, hopefully, will allow me to avoid the biggest
///     issue I have with this class and its method - the desire to have both called the same thing...
/// </summary>
public static class MapPostEndpoint
{
    /// <summary>
    ///     As the name suggests, this method will map the Files (Post) endpoint specifically
    /// </summary>
    /// <param name="endpointRouteBuilder"></param>
    public static void MapFilesPostEndpoint(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var versionedApi = endpointRouteBuilder.NewVersionedApi(EndpointConstants.AddFilesGroupName);

        var apiGroup = versionedApi
                       .MapGroup(EndpointConstants.AddFilesEndpoint)
                       .HasApiVersion(1.0);

        apiGroup.MapPost("/", async (AddFilesRequest   files, [FromServices] FilesContext filesContext, ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken)
                                  => await PostedFiles.Handle(files, filesContext, TimeProvider.System, claimsPrincipal.Identity?.Name ?? "Jay Barden", cancellationToken))
                .Produces<IReadOnlyCollection<AddFilesResponse>>(201)
                .Produces(401)
                .Produces(403);
    }
}
