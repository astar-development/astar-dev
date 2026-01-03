using System.IO.Abstractions.TestingHelpers;
using AStar.Dev.OneDrive.Client.ApplicationConfiguration;
using AStar.Dev.OneDrive.Client.Login;
using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.User;
using AStar.Dev.OneDrive.Client.ViewModels;
using AStar.Dev.OneDrive.Client.Views;
using AStar.Dev.Utilities;
using Avalonia;
using Avalonia.Controls;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AStar.Dev.OneDrive.Client.Tests.Unit.Views;

[Collection("AvaloniaApp")]
public class MainWindowCoordinatorShould
{
    private const string UserPreferencesFilePath = "/home/jason/Documents/.config/astar-dev/AStar.OneDrive.Client.settings.json";

    static MainWindowCoordinatorShould() => AppHelper.EnsureHeadlessAvaloniaApp();

    private static (UserPreferenceService ups, MockFileSystem fs) CreateServicesWith(MockFileSystem? fs = null)
    {
        MockFileSystem fileSystem = fs ?? new MockFileSystem();
        var appSettings = new ApplicationSettings { UserPreferencesPath = UserPreferencesFilePath };
        var ups = new UserPreferenceService(fileSystem, appSettings);
        return (ups, fileSystem);
    }

    [Fact]
    public void MainWindowCoordinatorShouldInitializeTheViewModelAndWindowFromStoredPreferences()
    {
        IOneDriveService? mockOneDriveService = Substitute.For<IOneDriveService>();
        AppHelper.EnsureHeadlessAvaloniaApp();
        var fs = new MockFileSystem();
        var seed = new UserPreferences
        {
            WindowSettings = new WindowSettings
            {
                WindowWidth = 1200,
                WindowHeight = 800,
                WindowX = 50,
                WindowY = 75
            },
            UiSettings = new UiSettings
            {
                Theme = "Dark",
                RememberMe = false,
                FollowLog = false,
                DownloadFilesAfterSync = true,
                LastAction = "Previously did something"
            }
        };
        fs.AddFile(UserPreferencesFilePath, new MockFileData(seed.ToJson()));

        (UserPreferenceService ups, _) = CreateServicesWith(fs);
        var themeService = new ThemeService();
        var sut = new MainWindowCoordinator(ups, themeService);

        ILogger<MainWindowViewModel>? logger = Substitute.For<ILogger<MainWindowViewModel>>();
        var vm = new MainWindowViewModel(mockOneDriveService, logger);
        var window = new Window();

        // Act
        sut.Initialize(window, vm);

        // Assert
        vm.Status.ShouldBe("Previously did something");
        vm.UserPreferences.UiSettings.Theme.ShouldBe("Dark");
        vm.UserPreferences.UiSettings.RememberMe.ShouldBeFalse();
        vm.UserPreferences.UiSettings.FollowLog.ShouldBeFalse();
        vm.UserPreferences.UiSettings.DownloadFilesAfterSync.ShouldBeTrue();

        window.Width.ShouldBe(1200);
        window.Height.ShouldBe(800);
        window.Position.X.ShouldBe(50);
        window.Position.Y.ShouldBe(75);
    }

    [Fact]
    public void MainWindowCoordinatorShouldPersistWindowAndUiSettingsToStorage()
    {
        IOneDriveService? mockOneDriveService = Substitute.For<IOneDriveService>();
        AppHelper.EnsureHeadlessAvaloniaApp();
        var fs = new MockFileSystem();
        // Start with defaults in storage
        var initial = new UserPreferences();
        fs.AddFile(UserPreferencesFilePath, new MockFileData(initial.ToJson()));

        (UserPreferenceService ups, _) = CreateServicesWith(fs);
        var themeService = new ThemeService();
        var sut = new MainWindowCoordinator(ups, themeService);

        ILogger<MainWindowViewModel>? logger = Substitute.For<ILogger<MainWindowViewModel>>();
        var vm = new MainWindowViewModel(mockOneDriveService, logger)
        {
            UserPreferences = new UserPreferences
            {
                UiSettings = new UiSettings
                {
                    RememberMe = false,
                    FollowLog = false,
                    Theme = "Light",
                    DownloadFilesAfterSync = true
                }
            }
        };

        var window = new Window
        {
            Width = 1600,
            Height = 900,
            Position = new PixelPoint(300, 400)
        };

        // Act
        sut.PersistUserPreferences(window, vm);

        // Assert
        UserPreferences persisted = ups.Load();

        persisted.WindowSettings.WindowWidth.ShouldBe(1600);
        persisted.WindowSettings.WindowHeight.ShouldBe(900);
        persisted.WindowSettings.WindowX.ShouldBe(300);
        persisted.WindowSettings.WindowY.ShouldBe(400);

        persisted.UiSettings.RememberMe.ShouldBeFalse();
        persisted.UiSettings.FollowLog.ShouldBeFalse();
        persisted.UiSettings.Theme.ShouldBe("Light");
        persisted.UiSettings.DownloadFilesAfterSync.ShouldBeTrue();
    }

    [Fact]
    public void MainWindowCoordinatorShouldMapThemeToIndexValues()
    {
        (_, MockFileSystem fs) = CreateServicesWith();
        fs.AddFile(UserPreferencesFilePath, new MockFileData(new UserPreferences().ToJson()));
        var sut = new MainWindowCoordinator(
            new UserPreferenceService(fs, new ApplicationSettings { UserPreferencesPath = UserPreferencesFilePath }),
            new ThemeService());

        sut.MapThemeToIndex("Auto").ShouldBe(0);
        sut.MapThemeToIndex("Light").ShouldBe(1);
        sut.MapThemeToIndex("Dark").ShouldBe(2);
        sut.MapThemeToIndex("Unknown").ShouldBe(0);
    }

    [Fact]
    public void MainWindowCoordinatorShouldMapIndexToThemeValues()
    {
        (_, MockFileSystem fs) = CreateServicesWith();
        fs.AddFile(UserPreferencesFilePath, new MockFileData(new UserPreferences().ToJson()));
        var sut = new MainWindowCoordinator(
            new UserPreferenceService(fs, new ApplicationSettings { UserPreferencesPath = UserPreferencesFilePath }),
            new ThemeService());

        sut.MapIndexToTheme(0).ShouldBe("Auto");
        sut.MapIndexToTheme(1).ShouldBe("Light");
        sut.MapIndexToTheme(2).ShouldBe("Dark");
        sut.MapIndexToTheme(999).ShouldBe("Auto");
    }
}
