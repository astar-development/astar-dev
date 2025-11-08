using AStar.Dev.Annotations;

namespace Demo.Domain.Entities;

[RegisterService(Lifetime.Singleton)]
public class DemoServiceWithInterfaceSingleton : IDemoService2
{
    
}

public interface IDemoService2
{
    
}
