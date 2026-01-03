using System.IO.Abstractions;
using AStar.Dev.OneDrive.Client.ApplicationConfiguration;
using AStar.Dev.Utilities;

namespace AStar.Dev.OneDrive.Client.User;

public class UserPreferenceService(IFileSystem fileSystem, ApplicationSettings appSettings)
{
    public UserPreferences Load()
        => fileSystem.File.ReadAllText(appSettings.UserPreferencesPath).FromJson<UserPreferences>();

    public void Save(UserPreferences userPreferences) => fileSystem.File.WriteAllText(appSettings.UserPreferencesPath, userPreferences.ToJson());
}
