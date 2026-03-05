using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetAll.V1;

public sealed class GetAllScrapeDirectoriesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ScrapeDirectoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ScrapeDirectoriesEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/",
                   async (HttpContext context, AdminContext adminContext, ILogger<GetAllScrapeDirectoriesEndpoint> logger, CancellationToken cancellationToken) =>
                       await DoStuff(new GetAllScrapeDirectoriesQuery(context.User), adminContext, logger, cancellationToken))
           .AddBasicProduces<IEnumerable<GetAllScrapeDirectoriesQueryResponse>>()
           .WithDescription("Get all scrape directories - shared across all sites")
           .WithSummary("Get all scrape directories")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.ScrapeDirectoriesTag);
    }

    private static async Task<IResult> DoStuff(GetAllScrapeDirectoriesQuery getAllScrapeDirectoriesQuery,
                                        AdminContext adminContext,
                                        ILogger<GetAllScrapeDirectoriesEndpoint> logger,
                                        CancellationToken cancellationToken)
    {
        logger.LogDebug("Getting all scrape directories by {User}", getAllScrapeDirectoriesQuery.User.Identity?.Name);

        List<ScrapeDirectory> scrapeDirectories = await adminContext
                                                       .ScrapeDirectories.GroupBy(l => l.ScrapeDirectoryId)
                                                       .Select(g => g.OrderByDescending(c => c.UpdatedOn).First())
                                                       .ToListAsync(cancellationToken);

        var data = scrapeDirectories
                  .Select(scrapeDirectory => new GetAllScrapeDirectoriesQueryResponse(scrapeDirectory))
                  .ToList();

        logger.LogDebug("Returning all scrape directories");

        return TypedResults.Ok(data);
    }
}
