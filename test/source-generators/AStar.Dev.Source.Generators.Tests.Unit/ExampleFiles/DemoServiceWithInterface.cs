using AStar.Dev.Annotations;

namespace Demo.Domain.Entities;

[RegisterService]
public class DemoServiceWithInterface : IDemoService
{
    
}

public interface IDemoService
{
    
}