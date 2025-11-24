using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AStar.Dev.OneDrive.Client.Services;

public static class AppPathHelper
{
    public static string GetAppDataPath(string appName)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var baseDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(baseDir, appName);
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDir, "Library", "Application Support", appName);
        }
        else // Linux and other Unix
        {
            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(homeDir, ".config", appName);
        }
    }
}
