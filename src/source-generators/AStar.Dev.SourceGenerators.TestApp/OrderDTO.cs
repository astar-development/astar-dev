using AStar.Dev.Annotations;

namespace AStar.Dev.SourceGenerators.TestApp;

[MapFrom(typeof(Order))]
public sealed class OrderDto
{
    public OrderId Id { get; set; } // same type → direct assign
    public string Status { get; set; } = "";
    public int Quantity { get; set; }
    public string? Note { get; set; } // no source → generator will DIAGNOSE
}
