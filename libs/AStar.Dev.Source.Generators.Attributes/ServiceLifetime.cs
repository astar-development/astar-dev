namespace AStar.Dev.Source.Generators.Attributes;

/// <summary>
/// Specifies the lifetime of a service within a dependency injection container.
/// </summary>
public enum ServiceLifetime
{
    /// <summary>
    /// A single instance is created and shared throughout the application's lifetime.
    /// </summary>
    Singleton,
    /// <summary>
    /// A new instance is created for each scope. Typically used for web requests.
    /// </summary>
    Scoped,
    /// <summary>
    /// A new instance is created every time it is requested.
    /// </summary>
    Transient
}
