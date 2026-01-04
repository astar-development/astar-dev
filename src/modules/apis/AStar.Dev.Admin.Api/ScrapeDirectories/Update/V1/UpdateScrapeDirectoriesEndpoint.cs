using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.ScrapeDirectories.Update.V1;

/// <summary>
///     The <see cref="UpdateScrapeDirectoriesEndpoint" /> class contains the <see cref="AddEndpoint" /> method.
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class UpdateScrapeDirectoriesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.ScrapeDirectoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.ScrapeDirectoriesEndpoint)
                                    .HasDeprecatedApiVersion(0.9)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapPut("/{scrapeDirectoryId:guid}",
                   async ([FromRoute] Guid scrapeDirectoryId,
                          [FromBody] UpdateScrapeDirectoriesCommand updateScrapeDirectoriesCommand,
                          HttpContext context,
                          AdminContext adminContext,
                          CancellationToken cancellationToken) => await Handle(new UpdateScrapeDirectoriesCommandForDb(scrapeDirectoryId, context.User, updateScrapeDirectoriesCommand), adminContext,
                                                                               cancellationToken))
           .AddBasicWithAdditionalProduces<UpdateScrapeDirectoriesResponse>()
           .WithDescription("Update the scrape directories used by the scrape APIs")
           .WithSummary("Update the Scrape Directories")
           .WithName("UpdateScrapeDirectories")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.ScrapeDirectoriesTag)
           .MapToApiVersion(1.0);
    }

    private async Task<IResult> Handle(UpdateScrapeDirectoriesCommandForDb request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        ScrapeDirectory? scrapeDirectory = await adminContext
                                                .ScrapeDirectories
                                                .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                                .Where(site => site.ScrapeDirectoryId == request.ScrapeDirectoryId)
                                                .FirstOrDefaultAsync(cancellationToken);

        if(scrapeDirectory is null) return TypedResults.NotFound();

        var insertConfig = new ScrapeDirectory
                           {
                               BaseDirectoryFamous = request.BaseDirectoryFamous,
                               UpdatedOn           = DateTime.UtcNow,
                               UpdatedBy           = request.UpdatedBy,
                               BaseSaveDirectory   = request.BaseSaveDirectory,
                               BaseDirectory       = request.BaseDirectory,
                               RootDirectory       = request.RootDirectory,
                               SubDirectoryName    = request.SubDirectoryName,
                               ScrapeDirectoryId   = request.ScrapeDirectoryId,
                           };

        _ = adminContext.ScrapeDirectories.Add(insertConfig);
        _ = await adminContext.SaveChangesAsync(cancellationToken);

        var apiVersion = new ApiVersion { Version = "1.0", };

        return TypedResults.CreatedAtRoute(new UpdateScrapeDirectoriesResponse(insertConfig), "GetScrapeDirectoriesById", apiVersion);
    }
}
