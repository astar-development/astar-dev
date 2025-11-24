using AStar.Dev.Annotations;

namespace Demo.Domain.Entities;

[RegisterService]
public sealed class DemoServiceWithInterface : IDemoService
{
    
}

public interface IDemoService
{
    
}