using AStar.Dev.Files.Api.Client.SDK.Models;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileAccessDetailShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new FileAccessDetail
            {
                DetailsLastUpdated = new DateTime(2025, 12, 23) ,
                HardDeletePending  = true,
                Id                 = 1,
                LastViewed         = new DateTime(2025, 12, 24),
                MoveRequired       = true,
                SoftDeleted        = true,
                SoftDeletePending  = true
            }.ToString()
             .ShouldMatchApproved();
}
