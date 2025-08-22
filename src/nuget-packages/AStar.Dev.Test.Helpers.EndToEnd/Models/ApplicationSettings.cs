using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace AStar.Dev.Test.Helpers.EndToEnd.Models;

/// <summary>
///     The <see cref="ApplicationSettings" /> class defines the default application settings that the EndToEnd tests require
///     <para>
///         This class has not been sealed as the intention is to leave it open for extension should your project require it
///     </para>
/// </summary>
public class ApplicationSettings
{
    private string _baseUri;

    /// <summary>
    ///     Gets the Configuration Section Name - i.e. the name of the section that must exist in the AppSettings.json - it exists to avoid the use of 'magic strings'.
    ///     <para>The value is 'ApplicationSettings'</para>
    /// </summary>
    public static string ConfigurationSectionName => "ApplicationSettings";

    /// <summary>
    ///     Gets or sets the BaseUri of the UI being tested - if it ends with a /, the get will remove to ensure combinations visually make sense. e.g.:
    ///     <para>
    ///         BaseUri + "/some-uri" (Please don't use + though ;-)!)
    ///     </para>
    /// </summary>
    [Required]
    public required string BaseUri
    {
        get => _baseUri = _baseUri.EndsWith('/') ? _baseUri[..^1] : _baseUri;
        [MemberNotNull(nameof(_baseUri))]
        set => _baseUri = value;
    }

    /// <summary>
    ///     Gets or sets the Use Headless setting that, as you might expect, controls whether the browser is opened 'headed' (visible) or 'headless' (invisible). The default is <c>true</c>
    /// </summary>
    public bool UseHeadless { get; set; } = true;

    /// <summary>
    ///     Gets or sets the Home Page Title - intended to be used as a basic test parameter - i.e. to confirm the test has logged into the correct site
    /// </summary>
    [Required]
    public required string HomePageTitle { get; set; }
}
