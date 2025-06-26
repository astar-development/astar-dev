using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Web.UI.Models;

public class ApplicationSettings
{
    internal static string ConfigurationSectionName => "ApplicationSettings";

    [Required]
    public required string BaseUri { get; set; }

    public bool UseHeadless { get; set; }

    public bool IsDevelopment { get; set; }

    [Required]
    public required string HomePageTitle { get; set; }
}
