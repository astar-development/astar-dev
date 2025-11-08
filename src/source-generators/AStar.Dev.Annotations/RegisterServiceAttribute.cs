namespace AStar.Dev.Annotations;

public enum Lifetime
{
    Singleton,
    Scoped,
    Transient
}

/// <summary>
/// An attribute used to register a class as a service within a dependency injection container.
/// The attribute allows specifying the service's lifetime, the interface it should be registered against,
/// and whether the concrete type should also be registered as itself.
/// </summary>
/// <param name="lifetime">The required lifetime of the service being registered</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterServiceAttribute(Lifetime lifetime = Lifetime.Scoped) : Attribute
{
    /// <summary>
    /// Specifies the lifetime of a service when registered in a dependency injection container.
    /// </summary>
    /// <remarks>
    /// Lifetime defines how the service is instantiated and shared within the application:
    /// - Singleton: A single instance is created and shared across the entire application.
    /// - Scoped: A single instance is created and shared within the same scope, such as a single HTTP request in web applications.
    /// - Transient: A new instance is created each time the service is requested.
    /// </remarks>
    public Lifetime Lifetime { get; } = lifetime;

    /// <summary>
    /// Specifies the interface or base type that the service should be registered against
    /// within the dependency injection container.
    /// </summary>
    /// <remarks>
    /// This property allows for explicitly defining the type the service is associated with,
    /// enabling more control over how the service is resolved at runtime. If not specified,
    /// the default behavior may vary based on the DI container implementation.
    /// </remarks>
    public Type? As { get; set; }

    /// <summary>
    /// Determines whether the concrete type of the service should be registered as itself
    /// in addition to any specified service interface.
    /// </summary>
    /// <remarks>
    /// If set to <c>true</c>, the concrete type will be registered in the dependency injection
    /// container so that it can be resolved directly. This is useful when the service
    /// needs to be resolved by its own type rather than an interface.
    /// </remarks>
    public bool AsSelf { get; set; } = false;
}
