using AStar.Dev.OneDrive.Client.Services;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AStar.Dev.OneDrive.Client.Theme;

public sealed class ThemeService
{
    public void PersistNewTheme(ComboBox cb, App app, UserSettingsService userSettingsService)
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
            UserSettings s = userSettingsService.Load();
            s.Theme = cb.SelectedIndex switch
            {
                1 => "Light",
                2 => "Dark",
                _ => "Auto"
            };

            userSettingsService.Save(s);
        }
        catch
        {
            // ignored
        }
    }

    public void ApplyThemePreference(UserSettings userSettings)
    {
        if(Application.Current is not App app) return;

        switch(userSettings.Theme)
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
