using AStar.Dev.Utilities;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(DuplicatesList))]
public class DuplicatesListShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new DuplicatesList(new(123456, 987, 654),
                              [
                                  new(1, 2, "MockFileName", "MockDirectoryName", 12, 345, 543, "MockFileHandle", true, 32, DateTimeOffset.MaxValue, DateTimeOffset.MaxValue.AddDays(-1),
                                      true, true, true, true)
                              ])
           .ToJson()
           .ShouldMatchApproved();
}
