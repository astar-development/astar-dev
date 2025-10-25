using AStar.Dev.Database.Updater.Core.Models;
using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Core.Tests.Unit.Models;

[TestSubject(typeof(ApiConfiguration))]
public class ApiConfigurationShould
{
    [Fact]
    public void ContainTheExpectedProperties() => new ApiConfiguration().ToJson().ShouldMatchApproved();

    [Fact]
    public void OverrideTheToStringMethodAsExpected() => new ApiConfiguration().ToString().ShouldMatchApproved();
}
