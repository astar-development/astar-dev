using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AStar.Dev.Database.Updater;

/// <summary>
///     Provides extension methods for configuring instances of <see cref="JsonSerializerOptions" />
///     with predefined serialization settings.
/// </summary>
public static class JsonSerializerOptionsExtensions
{
    /// <summary>
    ///     Configures the specified <see cref="JsonSerializerOptions" /> with predefined settings such as property naming policy,
    ///     handling for null values, reference handling, and more.
    /// </summary>
    /// <param name="jsonSerializerOptions">The <see cref="JsonSerializerOptions" /> instance to configure.</param>
    public static void CreateJsonConfigureOptions(JsonSerializerOptions jsonSerializerOptions)
    {
        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        jsonSerializerOptions.WriteIndented = true;
        jsonSerializerOptions.Encoder = JavaScriptEncoder.Default;
        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    }
}
