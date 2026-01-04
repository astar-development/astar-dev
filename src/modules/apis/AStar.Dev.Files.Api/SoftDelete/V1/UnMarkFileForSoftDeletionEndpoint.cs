using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Infrastructure.FilesDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.SoftDelete.V1;

public sealed class UnMarkFileForSoftDeletionEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SoftDeleteGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SoftDeleteUnMarkEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapPut("/",
                   async (UnMarkFileForSoftDeletionCommand           fileId,
                          FilesContext                               filesContext,
                          ILogger<UnMarkFileForSoftDeletionEndpoint> logger,
                          CancellationToken                          cancellationToken) => await Handle(fileId, filesContext, logger, cancellationToken))
           .AddBasicProduces<UnMarkFileForSoftDeletionCommandResponse>()
           .WithDescription("UnMarkFileForSoftDeletionEndpoint")
           .WithSummary("UnMarkFileForSoftDeletionEndpoint")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.SoftDeleteTag);
    }

    private async Task<IResult> Handle(UnMarkFileForSoftDeletionCommand fileId, FilesContext filesContext, ILogger<UnMarkFileForSoftDeletionEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("UnMark for soft deletion requested for fileId: {FileId}", fileId.FileId);

        FileDetail? details = await filesContext.Files.Include(f => f.FileAccessDetail).Include(fileDetail => fileDetail.DeletionStatus)
                                                .SingleOrDefaultAsync(f => f.Id == new FileId(fileId.FileId), cancellationToken);

        if(details is null)
        {
            logger.LogDebug("UnMark for soft deletion requested for fileId: {FileId} - Not found", fileId.FileId);

            return Results.NotFound();
        }

        details.DeletionStatus.SoftDeletePending = null;
        await filesContext.SaveChangesAsync(cancellationToken);

        logger.LogDebug("UnMark for soft deletion requested for fileId: {FileId} completed successfully", fileId.FileId);

        return Results.Ok(new UnMarkFileForSoftDeletionCommandResponse(fileId.FileId));
    }
}
