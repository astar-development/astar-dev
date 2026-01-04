using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Classifications.GetAllForFile.V1;

public sealed class GetAllClassificationsForFileEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ClassificationsGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ClassificationsEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/{handle}",
                   async (string handle, FilesContext filesContext, ILogger<GetAllClassificationsForFileEndpoint> logger, CancellationToken cancellationToken) =>
                       await Handle(handle, filesContext, logger, cancellationToken))
           .Produces<IEnumerable<string>>()
           .Produces(401)
           .Produces(500)
           .WithDescription("Gets all current File Classifications for the specified file handle")
           .WithSummary("Get the current File Classifications for a file")

            //// .RequireAuthorization()
           .WithTags(EndpointConstants.ClassificationsTag);
    }

    private static async Task<IResult> Handle(string handle, FilesContext filesContext, ILogger<GetAllClassificationsForFileEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting all File Classifications starting from: {FileHandle}", handle);

        List<FileDetail> classifications = await filesContext.Files
                                                             .Include(f => f.FileClassifications).Where(x => x.FileHandle == handle)
                                                             .ToListAsync(cancellationToken);

        logger.LogDebug("Returning all File Classifications ({FileHandle})", handle);

        return TypedResults.Ok(classifications);
    }
}
