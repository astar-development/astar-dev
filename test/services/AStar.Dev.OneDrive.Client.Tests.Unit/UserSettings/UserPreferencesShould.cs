using AStar.Dev.OneDrive.Client.UserSettings;
using AStar.Dev.Utilities;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.UserSettings;

public class UserPreferencesShould
{
    [Fact]
    public void ContainTheExpectedProperties() => new UserPreferences().ToJson().ShouldMatchApproved();
}
