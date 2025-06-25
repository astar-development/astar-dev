namespace AStar.Dev.Images.Api.Client.SDK.Models;

/// <summary>
///     The <see cref="NotFound" /> class that can be used to supply an image for the "not found" event
/// </summary>
public static class NotFound
{
    /// <summary>
    ///     Gets the <see cref="byte" /> array representing the "not found" image
    /// </summary>
    public static byte[] Image => File.ReadAllBytes("/app/wwwroot/assets/404.jpg");
}
