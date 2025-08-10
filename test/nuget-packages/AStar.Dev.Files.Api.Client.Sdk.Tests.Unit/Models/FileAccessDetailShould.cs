namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileAccessDetailShould
{
    [Fact]
    public void ReturnTheExpectedToString() =>
        new FileAccessDetail
            {
                DetailsLastUpdated = new DateTime(2025, 12, 23, 12, 34, 56, 789, DateTimeKind.Utc),
                HardDeletePending  = true,
                Id                 = 1,
                LastViewed         = new DateTime(2025, 12, 24, 12, 34, 56, 789, DateTimeKind.Utc),
                MoveRequired       = true,
                SoftDeleted        = true,
                SoftDeletePending  = true
            }.ToString()
             .ShouldMatchApproved();
}
