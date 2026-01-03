using AStar.Dev.OneDrive.Client.ApplicationConfiguration;
using AStar.Dev.Utilities;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.ApplicationConfiguration;

public class ApplicationSettingsShould
{
    [Fact]
    public void ContainTheExpectedPropertiesWithTheExpectedValues()
        => new ApplicationSettings()
            .ToJson()
            .ShouldMatchApproved();
}
