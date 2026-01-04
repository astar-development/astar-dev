using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SearchCategories.GetById.V1;

/// <summary>
///     The <see cref="GetSearchCategoriesByIdEndpoint" /> class
/// </summary>
/// <param name="app"></param>
public sealed class GetSearchCategoriesByIdEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SearchCategoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SearchCategoriesEndpoint)
                                    .HasApiVersion(1.0);

        apiGroup
           .MapGet("/{searchCategoryId:guid}",
                   async (Guid searchCategoryId, HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetSearchCategoriesByIdQuery(searchCategoryId, context.User), adminContext, cancellationToken))
           .WithDescription("This method returns a single set of search categories by the specified id")
           .WithSummary("Get the search categories by id")
           .WithName("GetSearchCategoriesById")

            // .RequireAuthorization()
           .AddBasicProduces<GetSearchCategoriesByIdResponse>()
           .Produces(404)
           .WithTags(EndpointConstants.SearchCategoriesTag);
    }

    private async Task<IResult> Handle(GetSearchCategoriesByIdQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        SearchCategory? searchCategory = await adminContext
                                              .SearchCategory
                                              .OrderByDescending(siteConfig => siteConfig.UpdatedOn)
                                              .Where(searchCategory => searchCategory.SearchCategoryId == request.SearchCategoryId)
                                              .FirstOrDefaultAsync(cancellationToken);

        return searchCategory is null
                   ? Results.NotFound()
                   : Results.Ok(new GetSearchCategoriesByIdResponse(searchCategory));
    }
}
