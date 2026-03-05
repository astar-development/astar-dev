using AStar.Dev.Infrastructure.AdminDb.Models;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Admin.Api.SearchCategories.GetById.V1;

[TestSubject(typeof(GetSearchCategoriesByIdResponse))]
public sealed class GetSearchCategoriesResponseShould
{
    [Fact]
    public void ContainTheExpectedValues()
    {
        var SearchCategories = new SearchCategory
                               {
                                   Name                = "Name",
                                   UpdatedBy           = "UpdatedBy",
                                   UpdatedOn           = new DateTime(2001, 1, 1),
                                   TotalPages          = 1,
                                   Id                  = 2,
                                   LastPageVisited     = 3,
                                   SearchCategoryId    = Guid.Empty,
                                   LastKnownImageCount = 4,
                                   Order               = 5,
                               };

        var sut = new GetSearchCategoriesByIdResponse(SearchCategories);

        sut.ToJson().ShouldMatchApproved();
    }
}
