namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     /// The <see cref="Option{T}" /> base class
/// </summary>
/// <typeparam name="T">The type of the parameter</typeparam>
public abstract class Option<T>
{
    /// <summary>
    ///     The implicit conversion operator to simplify creating a <see cref="None{T}" />
    /// </summary>
    /// <param name="_">Not used</param>
    public static implicit operator Option<T>(None _) =>
        new None<T>();

    /// <summary>
    ///     The implicit conversion operator to simplify creating a <see cref="Some{T}" />
    /// </summary>
    /// <param name="value">The value to assign to the <see cref="Some{T}" /></param>
    public static implicit operator Option<T>(T value) =>
        new Some<T>(value);
}
