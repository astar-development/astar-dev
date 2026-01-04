using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.SoftDelete.V1;

public sealed class MarkFileForSoftDeletionEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SoftDeleteGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SoftDeleteMarkEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapPut("/",
                   async (MarkFileForSoftDeletionCommand           fileId,
                          FilesContext                             filesContext,
                          ILogger<MarkFileForSoftDeletionEndpoint> logger,
                          CancellationToken                        cancellationToken) => await Handle(fileId, filesContext, logger, cancellationToken))
           .AddBasicProduces<MarkFileForSoftDeletionCommandResponse>()
           .WithDescription("MarkFileForSoftDeletionEndpoint")
           .WithSummary("MarkFileForSoftDeletionEndpoint")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.SoftDeleteTag);
    }

    private async Task<IResult> Handle(MarkFileForSoftDeletionCommand fileId, FilesContext filesContext, ILogger<MarkFileForSoftDeletionEndpoint> logger, CancellationToken cancellationToken)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1), cancellationToken);

        logger.LogDebug("Mark for soft deletion requested for fileId: {FileId}", fileId.FileId);

        FileDetail? details = await filesContext.Files.Include(f => f.FileAccessDetail).Include(fileDetail => fileDetail.DeletionStatus)
                                                .SingleOrDefaultAsync(f => f.Id == new FileId(fileId.FileId), cancellationToken);

        if(details is null)
        {
            logger.LogDebug("Mark for soft deletion requested for fileId: {FileId} - not found", fileId.FileId);

            return Results.NotFound();
        }

        details.DeletionStatus.SoftDeletePending = DateTimeOffset.UtcNow; // Get from DI
        await filesContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("Mark for soft deletion requested for fileId: {FileId} completed successfully", fileId.FileId);

        return Results.Ok(new MarkFileForSoftDeletionCommandResponse(fileId.FileId));
    }
}
