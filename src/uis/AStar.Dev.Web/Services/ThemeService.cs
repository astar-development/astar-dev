namespace AStar.Dev.Web.Services;

public class ThemeService
{
    private string       currentTheme = "dark";

    public string CurrentTheme
    {
        get => currentTheme;
        set
        {
            if (currentTheme == value)
            {
                return;
            }

            currentTheme = value;
            OnThemeChange?.Invoke();
        }
    }

    public event Action? OnThemeChange;
}
