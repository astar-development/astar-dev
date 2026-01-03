using AStar.Dev.OneDrive.Client.User;
using AStar.Dev.Utilities;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.User;

public class UserPreferencesShould
{
    [Fact]
    public void ContainTheExpectedPropertiesWithTheExpectedValues()
        => new UserPreferences()
            .ToJson()
            .ShouldMatchApproved();
}
