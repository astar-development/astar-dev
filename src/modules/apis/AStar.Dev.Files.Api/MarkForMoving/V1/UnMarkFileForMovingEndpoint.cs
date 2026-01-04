using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.MarkForMoving.V1;

public sealed class UnMarkFileForMovingEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.MoveGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.MoveUnMarkEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapPut("/",
                   async (UnMarkFileForMovingCommand           fileId,
                          FilesContext                         filesContext,
                          ILogger<UnMarkFileForMovingEndpoint> logger,
                          CancellationToken                    cancellationToken) => await Handle(fileId, filesContext, logger, cancellationToken))
           .AddBasicProduces<UnMarkFileForMovingCommandResponse>()
           .WithDescription("UnMarkFileForMovingEndpoint")
           .WithSummary("UnMarkFileForMovingEndpoint")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.MoveTag);
    }

    private async Task<IResult> Handle(UnMarkFileForMovingCommand fileId, FilesContext filesContext, ILogger<UnMarkFileForMovingEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("UnMark for moving of FileId: {FileId}", fileId.FileId);

        FileDetail? details = await filesContext.Files.Include(f => f.FileAccessDetail).SingleOrDefaultAsync(f => f.Id == new FileId(fileId.FileId), cancellationToken);

        if(details is null)
        {
            logger.LogDebug("UnMark for moving of FileId: {FileId} - not found", fileId.FileId);

            return Results.NotFound();
        }

        details.FileAccessDetail.MoveRequired = false;
        await filesContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("UnMark for moving of FileId: {FileId} completed successfully", fileId.FileId);

        return Results.Ok();
    }
}
