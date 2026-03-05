using Asp.Versioning.Builder;
using AStar.Dev.Infrastructure.AdminDb;
using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Minimal.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AStar.Dev.Admin.Api.SearchCategories.GetAll.V1;

/// <summary>
///     The <see cref="GetAllSearchCategoriesEndpoint" /> class
/// </summary>
/// <param name="app"></param>
public sealed class GetAllSearchCategoriesEndpoint(WebApplication app) : IEndpoint
{
    /// <inheritdoc />
    public void AddEndpoint()
    {
        IVersionedEndpointRouteBuilder versionedApi = app.NewVersionedApi(EndpointConstants.SearchCategoriesGroupName);

        RouteGroupBuilder apiGroup = versionedApi
                                    .MapGroup(EndpointConstants.SearchCategoriesEndpoint)
                                    .HasApiVersion(1.0);

        _ = apiGroup
           .MapGet("/",
                   async (HttpContext context, AdminContext adminContext, CancellationToken cancellationToken) =>
                       await Handle(new GetAllSearchCategoriesQuery(context.User), adminContext, cancellationToken))
           .AddBasicProduces<IEnumerable<GetAllSearchCategoriesResponse>>()
           .WithDescription("Get all available search categories")
           .WithSummary("Get all search categories")

           // .RequireAuthorization()
           .WithTags(EndpointConstants.SearchCategoriesTag);
    }

    private async Task<IResult> Handle(GetAllSearchCategoriesQuery request, AdminContext adminContext, CancellationToken cancellationToken)
    {
        List<SearchCategory> config = await adminContext
                                           .SearchCategory
                                           .ToListAsync(cancellationToken);

        IList<GetAllSearchCategoriesResponse> data = [];

        config.ForEach(config.Add);

        return TypedResults.Ok(data);
    }
}
