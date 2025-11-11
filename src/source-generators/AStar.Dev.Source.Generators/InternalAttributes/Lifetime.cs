namespace AStar.Dev.Source.Generators.InternalAttributes;

/// <summary>
///     Defines the lifetime of a service in a dependency injection container.
/// </summary>
/// <remarks>
///     This enumeration is used to specify how a service instance should be managed within a dependency injection
///     container:
///     - Singleton: A single instance is shared across the entire application.
///     - Scoped: A single instance is created per scope, such as per web request in web applications.
///     - Transient: A new instance is created every time the service is requested.
/// </remarks>
public enum Lifetime
{
    /// <summary>
    ///     Singleton: A single instance is shared across the entire application.
    /// </summary>
    Singleton,

    /// <summary>
    ///     Scoped: A single instance is created per scope, such as per web request in web applications.
    /// </summary>
    Scoped,

    /// <summary>
    ///     Transient: A new instance is created every time the service is requested.
    /// </summary>
    Transient
}
