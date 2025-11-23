using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace AStar.Dev.OneDrive.Client.Services;

public class UserSettings
{
    public string Theme { get; set; } = "Auto"; // Auto | Light | Dark
    public double WindowWidth { get; set; } = 800;
    public double WindowHeight { get; set; } = 600;
    public int? WindowX { get; set; }
    public int? WindowY { get; set; }
    public string? LastAccount { get; set; }
}

public class UserSettingsService
{
    private readonly string _filePath;

    public UserSettingsService()
    {
        var dir = GetSettingsDirectory();
        try
        {
            _ = Directory.CreateDirectory(dir);
        }
        catch { }

        _filePath = Path.Combine(dir, "AStar.OneDrive.Client.settings.json");
    }

    // Testable constructor: allow specifying custom file path (useful in unit tests)
    public UserSettingsService(string filePath) => _filePath = filePath;

    private static string GetSettingsDirectory()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AStarDev");
        }

        var xdg = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (!string.IsNullOrEmpty(xdg))
            return Path.Combine(xdg, "astar-dev");

        var home = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        // macOS and Linux default to ~/.config/astar-dev
        return Path.Combine(home, ".config", "astar-dev");
    }

    public UserSettings Load()
    {
        try
        {
            if (!File.Exists(_filePath))
                return new UserSettings();

            var json = File.ReadAllText(_filePath);
            UserSettings? s = JsonSerializer.Deserialize<UserSettings>(json);
            return s ?? new UserSettings();
        }
        catch
        {
            return new UserSettings();
        }
    }

    // Functional result-based loader
    public async Task<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>> LoadResultAsync() => await AStar.Dev.Functional.Extensions.Try.RunAsync(async () => Load());

    public void Save(UserSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
        catch { }
    }

    // Functional result-based saver (returns the saved settings on success)
    public async Task<AStar.Dev.Functional.Extensions.Result<UserSettings, Exception>> SaveResultAsync(UserSettings settings) => await AStar.Dev.Functional.Extensions.Try.RunAsync(async () =>
                                                                                                                                      {
                                                                                                                                          Save(settings);
                                                                                                                                          return settings;
                                                                                                                                      });
}
