using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Web.UI.Models;

public class UserDetails
{
    internal static string ConfigurationSectionName => "UserDetails";

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
