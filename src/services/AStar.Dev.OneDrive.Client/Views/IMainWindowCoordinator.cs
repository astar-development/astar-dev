using AStar.Dev.OneDrive.Client.Theme;
using AStar.Dev.OneDrive.Client.User;
using AStar.Dev.OneDrive.Client.ViewModels;
using Avalonia;
using Avalonia.Controls;

namespace AStar.Dev.OneDrive.Client.Views;

public interface IMainWindowCoordinator
{
    void Initialize(Window window, MainWindowViewModel vm);
    void PersistUserPreferences(Window window, MainWindowViewModel vm);

    int MapThemeToIndex(string theme);
    string MapIndexToTheme(int index);
}

public class MainWindowCoordinator(UserPreferenceService userPreferenceService, ThemeService themeService) : IMainWindowCoordinator
{
    public void Initialize(Window window, MainWindowViewModel vm)
    {
        UserPreferences userPreferences = userPreferenceService.Load();

        // Apply theme ASAP
        themeService.ApplyThemePreference(userPreferences);

        // Update VM
        vm.UserPreferences = userPreferences;
        vm.Status = userPreferences.UiSettings.LastAction;

        // Apply window position and size
        TryApplyWindow(window, userPreferences);
    }

    public void PersistUserPreferences(Window window, MainWindowViewModel vm)
    {
        UserPreferences userPreferencesToPersist = userPreferenceService.Load();

        userPreferencesToPersist.WindowSettings.WindowWidth = window.Width;
        userPreferencesToPersist.WindowSettings.WindowHeight = window.Height;
        userPreferencesToPersist.WindowSettings.WindowX = window.Position.X;
        userPreferencesToPersist.WindowSettings.WindowY = window.Position.Y;

        userPreferencesToPersist.UiSettings.RememberMe = vm.UserPreferences.UiSettings.RememberMe;
        userPreferencesToPersist.UiSettings.FollowLog = vm.UserPreferences.UiSettings.FollowLog;
        userPreferencesToPersist.UiSettings.Theme = vm.UserPreferences.UiSettings.Theme;
        userPreferencesToPersist.UiSettings.DownloadFilesAfterSync = vm.UserPreferences.UiSettings.DownloadFilesAfterSync;

        userPreferenceService.Save(userPreferencesToPersist);
    }

    public int MapThemeToIndex(string theme)
        => theme switch
        {
            "Light" => 1,
            "Dark" => 2,
            _ => 0
        };

    public string MapIndexToTheme(int index)
        => index switch
        {
            1 => "Light",
            2 => "Dark",
            _ => "Auto"
        };

    private static void TryApplyWindow(Window window, UserPreferences userPreferences)
    {
        try
        {
            if (userPreferences is { WindowSettings.WindowWidth: > 0, WindowSettings.WindowHeight: > 0 })
            {
                window.Width = userPreferences.WindowSettings.WindowWidth;
                window.Height = userPreferences.WindowSettings.WindowHeight;
            }

            if (userPreferences is { WindowSettings.WindowX: > 0, WindowSettings.WindowY: > 0 })
                window.Position = new PixelPoint(userPreferences.WindowSettings.WindowX, userPreferences.WindowSettings.WindowY);
        }
        catch
        {
            // ignored
        }
    }
}
