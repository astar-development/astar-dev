namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     The <see cref="None" /> object which is 1 half of the possible states of <see cref="Option{T}" />
/// </summary>
/// <typeparam name="T">The type of the parameter</typeparam>
public sealed class None<T> : Option<T>
{
    /// <summary>
    ///     Overrides the ToString method
    /// </summary>
    /// <returns>None - as a string</returns>
    public override string ToString() =>
        "None";
}
