namespace AStar.Dev.OneDrive.Client.User;

public class UserPreferences
{
    public WindowSettings WindowSettings { get; set; } = new();
    public UiSettings UiSettings { get; set; } = new();
}
