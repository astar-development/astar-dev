using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.GetById.V1;

/// <summary>
///     The <see cref="GetScrapeDirectoriesByIdEndpoint" /> class
/// </summary>
/// <param name="app"></param>
public sealed class GetScrapeDirectoriesByIdEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ScrapeDirectoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ScrapeDirectoriesEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/{scrapeDirectoryId}",
                   async (Guid scrapeDirectoryId, HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetScrapeDirectoriesByIdQuery(scrapeDirectoryId, context.User), adminContext, cancellationToken))
           .WithDescription("This method returns a single set of scrape directories by the specified site slug")
           .WithSummary("Get the scrape directories by site slug")
           .WithName("GetScrapeDirectoriesById")

           // .RequireAuthorization()
           .AddBasicProduces<GetScrapeDirectoriesByIdResponse>()
           .Produces(404)
           .WithTags(EndpointConstants.ScrapeDirectoriesTag);
    }

    private async Task<IResult> Handle(GetScrapeDirectoriesByIdQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        ScrapeDirectory? scrapeDirectory = await adminContext
                                                .ScrapeDirectories
                                                .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                                .Where(site => site.ScrapeDirectoryId == request.ScrapeDirectoryId)
                                                .FirstOrDefaultAsync(cancellationToken);

        return scrapeDirectory is null
                   ? Results.NotFound()
                   : Results.Ok(new GetScrapeDirectoriesByIdResponse(scrapeDirectory));
    }
}
