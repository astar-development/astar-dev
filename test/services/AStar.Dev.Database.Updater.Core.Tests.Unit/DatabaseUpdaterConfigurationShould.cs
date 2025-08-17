using AStar.Dev.Utilities;

namespace AStar.Dev.Database.Updater.Core;

[TestSubject(typeof(DatabaseUpdaterConfiguration))]
public class DatabaseUpdaterConfigurationShould
{
    [Fact]
    public void ContainTheExpectedProperties() =>
        new DatabaseUpdaterConfiguration
        {
            RootDirectory           = "Mock Root Directory",
            HonourFirstDelay        = true,
            MappingsFilePath        = "Mock Mappings File Path",
            SoftDeleteScheduledTime = TimeOnly.Parse("01:00:00"),
            HardDeleteScheduledTime = TimeOnly.Parse("02:00:00"),
            NewFilesScheduledTime   = TimeOnly.Parse("03:00:00")
        }.ToJson().ShouldMatchApproved();
}
