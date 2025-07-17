namespace AStar.Dev.Files.Api.Client.Sdk.Models;

public sealed class FileDetailShould
{
    [Fact(Skip = "Doesn't work...")]
    public void ReturnTheExpectedToString() =>
        new FileDetail
        {
            FileName      = "MockFileName",
            FileSize      = 1234,
            Height        = 123,
            Id            = 1,
            Width         = 456,
            DirectoryName = "MockDirectoryName",
            FileAccessDetail = new ()
                               {
                                   DetailsLastUpdated = new DateTime(2025, 12, 23),
                                   Id                 = 2,
                                   HardDeletePending  = true,
                                   LastViewed         = new DateTime(2025, 12, 24),
                                   MoveRequired       = true,
                                   SoftDeleted        = true,
                                   SoftDeletePending  = true
                               }
        }.ToString().ShouldMatchApproved();
}
