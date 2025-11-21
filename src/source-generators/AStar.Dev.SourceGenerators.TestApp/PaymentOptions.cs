using System.ComponentModel.DataAnnotations;
using AStar.Dev.Annotations;

namespace AStar.Dev.SourceGenerators.TestApp;

[ConfigSection("Payments")]
public sealed class PaymentsOptions
{
    public string ApiKey { get; set; } = default!;

    [Range(1, 120)] public int TimeoutSeconds { get; set; }
}
