using System.Text.Json;

namespace AStar.Dev.Usage.Logger;

public static class JsonSettings
{
    public static JsonSerializerOptions Options => new() { PropertyNameCaseInsensitive = true, };
}
