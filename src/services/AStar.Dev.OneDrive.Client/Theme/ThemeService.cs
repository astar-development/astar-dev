using AStar.Dev.OneDrive.Client.User;
using Avalonia;
using Avalonia.Styling;

namespace AStar.Dev.OneDrive.Client.Theme;

public class ThemeService
{
    public void ApplyThemePreference(UserPreferences userPreferences)
    {
        if (Application.Current is not App app) return;

        switch (userPreferences.UiSettings.Theme)
        {
            case "Light":
                app.SetTheme(ThemeVariant.Light);
                break;
            case "Dark":
                app.SetTheme(ThemeVariant.Dark);
                break;
            default:
                app.SetTheme(ThemeVariant.Default);
                break;
        }
    }
}
