using AStar.Dev.Utilities;

namespace AStar.Dev.Admin.Api.Client.Sdk.Models;

/// <summary>
///     The <see href="ModelToIgnore"></see> class.
/// </summary>
public sealed class ModelToIgnore
{
    /// <summary>
    ///     Gets or sets the value of the Model to ignore completely. I know, shocking...
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    ///     Returns this object in JSON format
    /// </summary>
    /// <returns>
    ///     This object serialized as a JSON object.
    /// </returns>
    public override string ToString() =>
        this.ToJson();
}
