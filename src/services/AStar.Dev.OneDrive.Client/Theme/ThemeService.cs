using AStar.Dev.OneDrive.Client.Services;
using AStar.Dev.OneDrive.Client.UserSettings;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AStar.Dev.OneDrive.Client.Theme;

public sealed class ThemeService
{
    public void PersistNewTheme(ComboBox cb, App app, UserPreferencesService userPreferencesService)
    {
        switch(cb.SelectedIndex)
        {
            case 1:
                app.SetTheme(ThemeVariant.Light);
                break;
            case 2:
                app.SetTheme(ThemeVariant.Dark);
                break;
            default:
                app.SetTheme(ThemeVariant.Default);
                break;
        }

        try
        {
            UserSettings.UserPreferences s = userPreferencesService.Load();
            s.Theme = cb.SelectedIndex switch
            {
                1 => "Light",
                2 => "Dark",
                _ => "Auto"
            };

            userPreferencesService.Save(s); // Pretty sure this is not required but it comes from a "working" version so not removing... yet
        }
        catch
        {
            // ignored
        }
    }

    public void ApplyThemePreference(UserSettings.UserPreferences userPreferences)
    {
        if(Application.Current is not App app) return;

        switch(userPreferences.Theme)
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
