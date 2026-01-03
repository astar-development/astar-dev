using System.Runtime.InteropServices;
using System.Text.Json;
using AStar.Dev.Functional.Extensions;

namespace AStar.Dev.OneDrive.Client.UserSettings;

public sealed class UserPreferencesService
{
    private readonly string _filePath;

    public UserPreferencesService()
    {
        var dir = GetSettingsDirectory();
        try
        {
            _ = Directory.CreateDirectory(dir);
        }
        catch { }

        _filePath = Path.Combine(dir, "AStar.OneDrive.Client.settings.json");
    }

    // Testable constructor: allows specifying the custom file path (useful in unit tests)
    public UserPreferencesService(string filePath) => _filePath = filePath;

    private static string GetSettingsDirectory()
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AStarDev");

        var xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if(!string.IsNullOrEmpty(xdg))
            return Path.Combine(xdg, "astar-dev");

        var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        // macOS and Linux default to ~/.config/astar-dev
        return Path.Combine(home, ".config", "astar-dev");
    }

    public UserPreferences Load()
    {
        try
        {
            if(!File.Exists(_filePath))
                return new UserPreferences();

            var json = File.ReadAllText(_filePath);
            UserPreferences? userSettings = JsonSerializer.Deserialize<UserPreferences>(json);
            return userSettings ?? new UserPreferences();
        }
        catch
        {
            return new UserPreferences();
        }
    }

    // Functional result-based loader
    public Result<UserPreferences, Exception> LoadResult() => Try.Run(Load);

    public void Save(UserPreferences preferences)
    {
        try
        {
            var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);

            // No explicit cache clearing API in Azure.Identity.
            // If RememberMe is false, the next run will construct the credential without persistence,
            // so tokens won't survive app exit. Well, that is what ChatGPT5 coded. I am not convinced...
        }
        catch
        {
            // swallow exceptions for now, but consider logging
        }
    }

    // Functional result-based saver (returns the saved settings on success)
    public Result<UserPreferences, Exception> SaveResult(UserPreferences preferences)
        =>  Functional.Extensions.Try.Run( () =>
                                                            {
                                                                Save(preferences);
                                                                return preferences;
                                                            });
}
