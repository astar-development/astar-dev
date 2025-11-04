using AStar.Dev.Utilities;

namespace AStar.Dev.Admin.Api.Client.Sdk.Models;

/// <summary>
///     The <see href="TagToIgnore"></see> class.
/// </summary>
public sealed class TagToIgnore
{
    /// <summary>
    ///     Gets or sets the value of the tag to ignore. I know, shocking...
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the Ignore Image property. When set to <c>true</c>, the image is ignored irrespective of any other
    ///     setting.
    /// </summary>
    public bool IgnoreImage { get; set; }

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>
    ///     This object serialized as a JSON object.
    /// </returns>
    public override string ToString()
        => this.ToJson();
}
