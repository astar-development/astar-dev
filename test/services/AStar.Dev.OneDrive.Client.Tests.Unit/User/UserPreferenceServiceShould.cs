using System.IO.Abstractions.TestingHelpers;
using AStar.Dev.OneDrive.Client.ApplicationConfiguration;
using AStar.Dev.OneDrive.Client.User;
using AStar.Dev.Utilities;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.User;

public class UserPreferenceServiceShould
{
    private const string UserPreferencesFilePath = "/home/jason/Documents/.config/astar-dev/AStar.OneDrive.Client.settings.json";
    private readonly ApplicationSettings _mockApplicationSettings = new() { UserPreferencesPath = UserPreferencesFilePath };

    [Fact]
    public void LoadTheUserPreferencesFromFile()
    {
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(UserPreferencesFilePath, new MockFileData(new UserPreferences
        {
            UiSettings = new UiSettings
            {
                FollowLog = true, DownloadFilesAfterSync = true, Theme = "Dark", LastAction = "Mock Action Set", RememberMe = true
            },
            WindowSettings = new WindowSettings
            {
                WindowHeight = 1234, WindowWidth = 5678, WindowX = 100, WindowY = 200
            }
        }.ToJson()));
        var sut = new UserPreferenceService(mockFileSystem, _mockApplicationSettings);

        UserPreferences settings = sut.Load();

        settings.ToJson().ShouldMatchApproved();
    }

    [Fact]
    public void SaveTheUserPreferencesFromFile()
    {
        var mockFileSystem = new MockFileSystem();
        mockFileSystem.AddFile(UserPreferencesFilePath, new MockFileData(new UserPreferences().ToJson()));
        var sut = new UserPreferenceService(mockFileSystem, _mockApplicationSettings);

        sut.Save(new UserPreferences
        {
            UiSettings = new UiSettings
            {
                FollowLog = true, DownloadFilesAfterSync = true, Theme = "Dark", LastAction = "Mock Action Set", RememberMe = true
            },
            WindowSettings = new WindowSettings
            {
                WindowHeight = 1234, WindowWidth = 5678, WindowX = 100, WindowY = 200
            }
        });

        mockFileSystem.File.ReadAllText(UserPreferencesFilePath).ShouldMatchApproved();
    }
}
