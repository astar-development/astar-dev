using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Files.Api.Classifications.GetAll.V1;

public sealed class GetAllClassificationsEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ClassificationsGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ClassificationsEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/",
                   async (FilesContext filesContext, ILogger<GetAllClassificationsEndpoint> logger, CancellationToken cancellationToken) => await Handle(filesContext, logger, cancellationToken))
           .Produces<IEnumerable<GetAllClassificationsResponse>>()
           .Produces(401)
           .Produces(500)
           .WithDescription("Gets all the current File Classifications that can be assigned to a file")
           .WithSummary("Get all current File Classifications")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.ClassificationsTag);
    }

    private async Task<IResult> Handle(FilesContext filesContext, ILogger<GetAllClassificationsEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting all file classifications");

        List<GetAllClassificationsResponse> fileClassifications = await filesContext.FileClassifications
                                                                                    .Select(fc => new GetAllClassificationsResponse { Id = fc.Id, Celebrity = fc.Celebrity, Name = fc.Name, })
                                                                                    .ToListAsync(cancellationToken);

        logger.LogDebug("Returning all file classifications");

        return TypedResults.Ok(fileClassifications);
    }
}
