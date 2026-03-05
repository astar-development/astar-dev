using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.HardDelete.V1;

public sealed class MarkFileForHardDeletionEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.HardDeleteGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.HardDeleteMarkEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapPut("/",
                   async (MarkFileForHardDeletionCommand fileId,
                          FilesContext filesContext,
                          ILogger<MarkFileForHardDeletionEndpoint> logger,
                          CancellationToken cancellationToken) => await Handle(fileId, filesContext, logger, cancellationToken))
           .AddBasicProduces<MarkFileForHardDeletionCommandResponse>()
           .WithDescription("MarkFileForHardDeletionEndpoint")
           .WithSummary("MarkFileForHardDeletionEndpoint")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.HardDeleteTag);
    }

    private async Task<IResult> Handle(MarkFileForHardDeletionCommand fileId, FilesContext filesContext, ILogger<MarkFileForHardDeletionEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Mark for hard deletion requested for fileId: {FileId}", fileId.FileId);

        FileDetail? details = await filesContext.Files.Include(f => f.FileAccessDetail).Include(fileDetail => fileDetail.DeletionStatus)
                                                .SingleOrDefaultAsync(f => f.Id == new FileId(fileId.FileId), cancellationToken);

        if(details is null)
        {
            logger.LogDebug("Mark for hard deletion requested for fileId: {FileId} - not found", fileId.FileId);

            return Results.NotFound();
        }

        details.DeletionStatus.HardDeletePending = DateTime.UtcNow; // need to get TimeProvide from DI

        _ = await filesContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Mark for hard deletion requested for fileId: {FileId} completed successfully", fileId.FileId);

        return Results.Ok();
    }
}
