using AStar.Dev.Files.Api.Client.SDK.Models;
using AStar.Dev.Utilities;
using JetBrains.Annotations;

namespace AStar.Dev.Files.Api.Client.Sdk.Models;

[TestSubject(typeof(FileSizeDetail))]
public class FileSizeDetailShould
{
    [Fact]
    public void ContainTheExpectedProperties()
        => new FileSizeDetail(123456, 789, 987).ToJson().ShouldMatchApproved();
}
