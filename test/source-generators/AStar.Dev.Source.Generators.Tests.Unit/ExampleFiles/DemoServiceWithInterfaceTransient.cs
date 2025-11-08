using AStar.Dev.Annotations;

namespace Demo.Domain.Entities;

[RegisterService(Lifetime.Transient)]
public class DemoServiceWithInterfaceTransient : IDemoService3
{
    
}

public interface IDemoService3
{
    
}
