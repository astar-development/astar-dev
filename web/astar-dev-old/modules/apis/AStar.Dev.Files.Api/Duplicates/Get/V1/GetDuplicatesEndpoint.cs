using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.FilesDb.Data;
using AStar.Dev.Minimal.Api.Extensions;
using AStar.Dev.Utilities;
using Z.Linq;

namespace AStar.Dev.Files.Api.Duplicates.Get.V1;

public sealed class GetDuplicatesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.DuplicatesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.DuplicatesEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/",
                   async ([AsParameters] GetDuplicatesQuery parameters,
                          FilesContext filesContext,
                          ILogger<GetDuplicatesEndpoint> logger,
                          CancellationToken cancellationToken) => await Handle(parameters, filesContext, logger, cancellationToken))
           .AddBasicProduces<IEnumerable<GetDuplicatesQueryResponse>>()
           .WithDescription("Gets all duplicate files matching the specified search criteria.")
           .WithSummary("Get all duplicate files")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.DuplicatesTag);
    }

    private async Task<IResult> Handle(GetDuplicatesQuery request, FilesContext filesContext, ILogger<GetDuplicatesEndpoint> logger, CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting duplicate files for: {SearchParameters}", request.ToJson());

        var scrapeDirectories = await filesContext.DuplicatesDetails
                                                  .FilterByDirectory(request.SearchFolder, request.Recursive)
                                                  .ExcludeAllButImages(request.SearchType)
                                                  .IncludeSoftDeleted(request.IncludeSoftDeleted)
                                                  .ExcludeViewed(request.ExcludeViewed)
                                                  .IncludeWhenContains(request.SearchText)
                                                  .SortBy(request.SortOrder)
                                                  .GroupBy(duplicatesDetails => new FileGrouping(duplicatesDetails.FileSize, duplicatesDetails.Height, duplicatesDetails.Width),
                                                           (grouping, duplicates) => new { fileSize = grouping, duplicates, })
                                                  .Skip((request.CurrentPage - 1) * request.ItemsPerPage).Take(request.ItemsPerPage)
                                                  .ToListAsync(cancellationToken);

        logger.LogDebug("Returning duplicate files count: {DuplicateFilesCount}", scrapeDirectories.Count);

        return TypedResults.Ok(scrapeDirectories);
    }
}
