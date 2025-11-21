using AStar.Dev.Annotations;

namespace AStar.Dev.SourceGenerators.TestApp;

public sealed class OrderDto
{
    public OrderId Id { get; set; } // same type → direct assign
    public string Status { get; set; } = "";
    public int Quantity { get; set; }
    public string? Note { get; set; } // no source → generator will DIAGNOSE
}

public partial class SampleEntity
{
    public int Id { get; } = 42;
    public string? Name { get; } = "Sample";
    public string? Name2 { get; } = "Sample";
}
