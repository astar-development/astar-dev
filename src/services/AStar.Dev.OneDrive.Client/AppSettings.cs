namespace AStar.Dev.OneDrive.Client;

public sealed class AppSettings
{
    public string ClientId { get; set; } = string.Empty;
    public string TenantId { get; set; } = "common"; // or your tenant
    public string ClientSecret { get; internal set; } = string.Empty;

}
