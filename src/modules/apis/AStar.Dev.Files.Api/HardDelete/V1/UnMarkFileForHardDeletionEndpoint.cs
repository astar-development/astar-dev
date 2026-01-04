using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.HardDelete.V1;

public sealed class UnMarkFileForHardDeletionEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.HardDeleteGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.HardDeleteUnMarkEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapPut("/",
                   async (UnMarkFileForHardDeletionCommand           fileId,
                          FilesContext                               filesContext,
                          ILogger<UnMarkFileForHardDeletionEndpoint> logger,
                          CancellationToken                          cancellationToken) => await Handle(fileId, filesContext, logger, cancellationToken))
           .AddBasicProduces<UnMarkFileForHardDeletionCommandResponse>()
           .WithDescription("UnMarkFileForHardDeletionEndpoint")
           .WithSummary("UnMarkFileForHardDeletionEndpoint")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.HardDeleteTag);
    }

    private async Task<IResult> Handle(UnMarkFileForHardDeletionCommand fileId, FilesContext filesContext, ILogger<UnMarkFileForHardDeletionEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("UnMark for hard deletion requested for fileId: {FileId}", fileId.FileId);

        FileDetail? details = await filesContext.Files.Include(f => f.FileAccessDetail).Include(fileDetail => fileDetail.DeletionStatus)
                                                .SingleOrDefaultAsync(f => f.Id == new FileId(fileId.FileId), cancellationToken);

        if(details is null)
        {
            logger.LogDebug("UnMark for hard deletion requested for fileId: {FileId} - not found", fileId.FileId);

            return Results.NotFound();
        }

        details.DeletionStatus.HardDeletePending = null;
        await filesContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("UnMark for hard deletion requested for fileId: {FileId} completed successfully", fileId.FileId);

        return Results.Ok();
    }
}
