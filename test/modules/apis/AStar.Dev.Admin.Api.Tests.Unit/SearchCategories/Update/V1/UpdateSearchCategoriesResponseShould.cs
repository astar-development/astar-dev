// using AStar.Dev.Utilities;
// using JetBrains.Annotations;
//
// namespace AStar.Dev.Admin.Api.SearchCategories.Update.V1;
//
// [TestSubject(typeof(UpdateSearchCategoryResponse))]
// public sealed class UpdateSearchCategoriesResponseShould
// {
//     [Fact]
//     public void ContainTheExpectedValues()
//     {
//         var sut = new UpdateSearchCategoryResponse(new()
//            {
//      Name      = "Name",
//      UpdatedBy = "UpdatedBy",
//      UpdatedOn = new(2001, 1, 1),
//      TotalPages          = 1,
//      Id        = 2,
//      LastPageVisited     = 3,
//      SearchCategoryId    = Guid.Empty,
//      LastKnownImageCount = 4,
//      Order     = 5,
//            });
//
//         sut.ToJson().ShouldMatchApproved();
//     }
// }

