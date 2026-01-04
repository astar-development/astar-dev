using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.MarkForMoving.V1;

public sealed class MarkFileForMovingEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.MoveGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.MoveMarkEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapPut("/",
                   async (MarkFileForMovingCommand           fileId,
                          FilesContext                       filesContext,
                          ILogger<MarkFileForMovingEndpoint> logger,
                          CancellationToken                  cancellationToken) => await Handle(fileId, filesContext, logger, cancellationToken))
           .AddBasicProduces<MarkFileForMovingCommandResponse>()
           .WithDescription("MarkFileForMovingEndpoint")
           .WithSummary("MarkFileForMovingEndpoint")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.MoveTag);
    }

    private async Task<IResult> Handle(MarkFileForMovingCommand fileId, FilesContext filesContext, ILogger<MarkFileForMovingEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mark for moving requested for fileId: {FileId}", fileId.FileId);

        FileDetail? details = await filesContext.Files.Include(f => f.FileAccessDetail).SingleOrDefaultAsync(f => f.Id == new FileId(fileId.FileId), cancellationToken);

        if(details is null)
        {
            logger.LogDebug("Mark for moving requested for fileId: {FileId} - Not found", fileId.FileId);

            return Results.NotFound();
        }

        details.FileAccessDetail.MoveRequired = true;
        await filesContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Mark for moving requested for fileId: {FileId} completed successfully", fileId.FileId);

        return Results.Ok();
    }
}
