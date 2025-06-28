using System.ComponentModel.DataAnnotations;

namespace AStar.Dev.Test.Helpers.EndToEnd.Models;

/// <summary>
///     The <see cref="UserDetails" /> class contains the default user details that are required, as a minimum, for the tests to run
///     <para>This class has not been sealed so that it can be extended if / when required by your tests</para>
/// </summary>
public class UserDetails
{
    /// <summary>
    ///     Gets the Configuration Section Name - i.e. the name of the section that must exist in the AppSettings.json - it exists to avoid the use of 'magic strings'.
    ///     <para>The value is 'UserDetails'</para>
    /// </summary>
    public static string ConfigurationSectionName => "UserDetails";

    /// <summary>
    ///     Gets or sets the Username to be used when a login is required
    /// </summary>
    [Required]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the password to be used when a login is required
    ///     <para>
    ///         This should be the real password. For tests that require an incorrect password, the setter is public so the password can be broken
    ///     </para>
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}
