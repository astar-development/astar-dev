namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     The <see cref="Option" /> class as the base of the discriminated union of <see cref="Some{T}" /> and <see cref="None{T}" />
/// </summary>
public static class Option
{
    /// <summary>
    ///     The extension method to optionally map the <see cref="Option{T}" /> to a <see cref="Some{T}" />
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="obj">The actual parameter - a runtime instance of <see cref="Some{T}" /> or <see cref="None{T}" /></param>
    /// <returns>Optionally a new <see cref="Some{T}" /></returns>
    public static Option<T> Optional<T>(this T obj) =>
        new Some<T>(obj);
}
