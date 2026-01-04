using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;

/// <summary>
///     The <see cref="UpdateSearchCategoryEndpoint" /> class contains the <see cref="AddEndpoint" /> method.
/// </summary>
/// <param name="app">The instance of <see cref="WebApplication" /> to add the endpoint to</param>
public sealed class UpdateSearchCategoryEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SearchCategoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SearchCategoriesEndpoint)
                                    .HasDeprecatedApiVersion(0.9)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapPut("/{searchCategoryId:guid}",
                   async ([FromRoute] Guid                        searchCategoryId,
                          [FromBody]  UpdateSearchCategoryCommand updateSearchCategoriesCommand,
                          HttpContext                             context,
                          AdminContext                            adminContext,
                          CancellationToken cancellationToken) => await Handle(new UpdateSearchCategoryCommandForDb(searchCategoryId, context.User, updateSearchCategoriesCommand), adminContext,
                                                                               cancellationToken))
           .AddBasicWithAdditionalProduces<UpdateSearchCategoryResponse>()
           .WithDescription("Updates the search category for the specified slug")
           .WithSummary("Update the search category")

            // .RequireAuthorization()
           .WithTags(EndpointConstants.SearchCategoriesTag)
           .MapToApiVersion(1.0);
    }

    private async Task<IResult> Handle(UpdateSearchCategoryCommandForDb request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        SearchCategory? config = await adminContext
                                      .SearchCategory
                                      .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                      .Where(site => site.SearchCategoryId == request.SearchCategoryId)
                                      .FirstOrDefaultAsync(cancellationToken);

        if(config is null) return TypedResults.NotFound();

        var insertConfig = new SearchCategory
                           {
                               SearchCategoryId    = request.SearchCategoryId,
                               UpdatedOn           = DateTime.UtcNow,
                               UpdatedBy           = request.UpdatedBy,
                               Name                = request.Name,
                               Order               = request.Order,
                               TotalPages          = request.TotalPages,
                               LastPageVisited     = request.LastPageVisited,
                               LastKnownImageCount = request.LastKnownImageCount,
                           };

        adminContext.SearchCategory.Add(insertConfig);
        await adminContext.SaveChangesAsync(cancellationToken);

        var apiVersion = new ApiVersion { Version = "1.0", };

        return TypedResults.CreatedAtRoute(new UpdateSearchCategoryResponse(insertConfig), "GetSearchCategoriesById", apiVersion);
    }
}
