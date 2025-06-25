namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     The <see cref="None" /> object which is 1 half of the possible states of <see cref="Option{T}" />
/// </summary>
public sealed class None
{
    /// <summary>
    ///     The empty value property
    ///     <para>Check why this was added</para>
    /// </summary>
    public static None Value { get; } = new();

    /// <summary>
    ///     A helper method to create a <see cref="None{T}" /> in a more fluent style
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <returns>An instance of <see cref="None{T}" /> where T is the specified type</returns>
    public static None<T> Of<T>() =>
        new();
}
