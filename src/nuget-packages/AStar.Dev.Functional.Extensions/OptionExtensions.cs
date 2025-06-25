namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     The <see cref="OptionExtensions" /> class containing the current extension for the <see cref="Option{T}" />
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    ///     The <see cref="Map{T,TResult}" /> to map the <see cref="Some{T}" /> to a new instance of <see cref="Some{TResult}" /> or <see cref="None{TResult}" /> if the original object was
    ///     <see cref="None{T}" />
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="obj">The actual parameter - a runtime instance of <see cref="Some{T}" /> or <see cref="None{T}" /></param>
    /// <param name="map"></param>
    /// <returns>The newly mapped <see cref="Some{TResult}" /> or new instance of <see cref="None{TResult}" /></returns>
    public static Option<TResult> Map<T, TResult>(this Option<T> obj, Func<T, TResult> map) =>
        obj is Some<T> some ? new Some<TResult>(map(some.Content)) : new None<TResult>();

    /// <summary>
    ///     The <see cref="Filter{T}" /> (aka WHERE) to filter teh <see cref="Some{T}" /> based on the supplied predicate
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="obj">The actual parameter - a runtime instance of <see cref="Some{T}" /> or <see cref="None{T}" /></param>
    /// <param name="predicate"></param>
    /// <returns>The original object when the runtime type is <see cref="Some{T}" /> and the object passes the predicate or an instance of <see cref="None{T}" /></returns>
    public static Option<T> Filter<T>(this Option<T> obj, Func<T, bool> predicate) =>
        obj is Some<T> some && !predicate(some.Content) ? new None<T>() : obj;

    /// <summary>
    ///     The <see cref="Reduce{T}(AStar.Dev.Functional.Extensions.Option{T},T)" /> to supply the final, default, object when the <see cref="Option{T}" /> is, in fact, <see cref="None{T}" />
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="obj">The actual parameter - a runtime instance of <see cref="Some{T}" /> or <see cref="None{T}" /></param>
    /// <param name="substitute">A substitute object to use when the runtime type is <see cref="None{T}" /></param>
    /// <returns>The underlying object when the runtime type is <see cref="Some{T}" /> or the substitute supplied when the runtime type is <see cref="None{T}" /></returns>
    public static T Reduce<T>(this Option<T> obj, T substitute) =>
        obj is Some<T> some ? some.Content : substitute;

    /// <summary>
    ///     The <see cref="Reduce{T}(AStar.Dev.Functional.Extensions.Option{T},Func{T})" /> to supply the final, default, object when the <see cref="Option{T}" /> is, in fact, <see cref="None{T}" />
    ///     <para>
    ///         This version differs from the <see cref="Reduce{T}(AStar.Dev.Functional.Extensions.Option{T},T)" /> version as the default object will not be created if the <see cref="Option{T}" /> is, in
    ///         fact, <see cref="Some{T}" />
    ///     </para>
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    /// <param name="obj">The actual parameter - a runtime instance of <see cref="Some{T}" /> or <see cref="None{T}" /></param>
    /// <param name="substitute">A function that can create the substitute object to use when the runtime type is <see cref="None{T}" /></param>
    /// <returns>The underlying object when the runtime type is <see cref="Some{T}" /> or the substitute created by the supplied function when the runtime type is <see cref="None{T}" /></returns>
    public static T Reduce<T>(this Option<T> obj, Func<T> substitute) =>
        obj is Some<T> some ? some.Content : substitute();
}
