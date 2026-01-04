namespace AStar.Dev.ToDo.Api.Controllers;

public sealed class ToDo
{
    public int Id { get; set; }

    public required string Title { get; set; }

    public required string? Owner { get; set; }
}
