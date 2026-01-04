using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SearchConfiguration.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchConfigurationEndpoint" /> class contains the <see cref="AddEndpoint" /> method.
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class UpdateSearchConfigurationEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SearchConfigurationGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SearchConfigurationEndpoint)
                                    .HasDeprecatedApiVersion(0.9)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapPut("/{slug}",
                   async ([FromRoute] string slug,
                          [FromBody] UpdateSearchConfigurationCommand updateSearchConfigurationCommand,
                          HttpContext context,
                          AdminContext adminContext,
                          CancellationToken cancellationToken) => await Handle(new UpdateSearchConfigurationCommandForDb(slug, context.User, updateSearchConfigurationCommand), adminContext,
                                                                               cancellationToken))
           .WithDescription("Update the search configuration matching the supplied slug")
           .WithSummary("Update the search configuration")
           .AddBasicWithAdditionalProduces<UpdateSearchConfigurationResponse>()
           .WithName("UpdateSearchConfiguration")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.SearchConfigurationTag)
           .MapToApiVersion(1.0);
    }

    private async Task<IResult> Handle(UpdateSearchConfigurationCommandForDb request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        Infrastructure.AdminDb.Models.SearchConfiguration? config = await adminContext
                                                                         .SearchConfiguration
                                                                         .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                                                         .Where(site => site.SiteConfigurationSlug == request.SiteConfigurationSlug)
                                                                         .FirstOrDefaultAsync(cancellationToken);

        if(config is null) return TypedResults.NotFound();

        var insertConfig = new Infrastructure.AdminDb.Models.SearchConfiguration
                           {
                               SearchString                    = request.SearchString,
                               TopWallpapers                   = request.TopWallpapers,
                               SearchStringPrefix              = request.SearchStringPrefix,
                               SearchStringSuffix              = request.SearchStringSuffix,
                               Subscriptions                   = request.Subscriptions,
                               ImagePauseInSeconds             = request.ImagePauseInSeconds,
                               StartingPageNumber              = request.StartingPageNumber,
                               TotalPages                      = request.TotalPages,
                               UseHeadless                     = request.UseHeadless,
                               SubscriptionsStartingPageNumber = request.SubscriptionsStartingPageNumber,
                               SubscriptionsTotalPages         = request.SubscriptionsTotalPages,
                               TopWallpapersTotalPages         = request.TopWallpapersTotalPages,
                               TopWallpapersStartingPageNumber = request.TopWallpapersStartingPageNumber,
                               SlowMotionDelayInMilliseconds   = request.SlowMotionDelayInMilliseconds,
                               SiteConfigurationSlug           = request.SiteConfigurationSlug,
                               UpdatedOn                       = DateTime.UtcNow,
                               UpdatedBy                       = request.UpdatedBy,
                           };

        _ = adminContext.SearchConfiguration.Add(insertConfig);
        _ = await adminContext.SaveChangesAsync(cancellationToken);

        var apiVersion = new ApiVersion { Version = "1.0", };

        return TypedResults.CreatedAtRoute(new UpdateSearchConfigurationResponse(insertConfig), "GetSearchConfigurationBySlug", apiVersion);
    }
}
