using System;
using System.Threading.Tasks;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Functional async extensions for working with <see cref="Task{Option}" />.
/// </summary>
public static class OptionAsyncExtensions
{
    /// <summary>
    ///     Asynchronously transforms the value inside an <see cref="Option{T}" /> if it exists.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="task">The asynchronous <see cref="Option{T}" /> to await and transform.</param>
    /// <param name="func">A mapping function to apply if a value is present.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> wrapping an <see cref="Option{TResult}" /> containing the transformed value, or <c>None</c>.
    /// </returns>
    public static async Task<Option<TResult>> MapAsync<T, TResult>(
        this Task<Option<T>> task,
        Func<T, TResult>     func)
    {
        var option = await task;

        return option is Option<T>.Some some
                   ? new Option<TResult>.Some(func(some.Value))
                   : Option.None<TResult>();
    }

    /// <summary>
    ///     Asynchronously pattern-matches on an <see cref="Option{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped value.</typeparam>
    /// <typeparam name="TResult">The return type of the match operation.</typeparam>
    /// <param name="task">The asynchronous <see cref="Option{T}" /> to await.</param>
    /// <param name="onSome">A function to invoke if a value is present.</param>
    /// <param name="onNone">A function to invoke if no value is present.</param>
    /// <returns>
    ///     A <see cref="Task{TResult}" /> containing the result of the match operation.
    /// </returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Option<T>>   task,
        Func<T, Task<TResult>> onSome,
        Func<Task<TResult>>    onNone)
    {
        var option = await task;

        return option is Option<T>.Some some
                   ? await onSome(some.Value)
                   : await onNone();
    }

    /// <summary>
    ///     Asynchronously binds the value to another <see cref="Task{Option}" /> if present.
    /// </summary>
    public static async Task<Option<TResult>> BindAsync<T, TResult>(
        this Task<Option<T>>           task,
        Func<T, Task<Option<TResult>>> func)
    {
        var option = await task;

        return option is Option<T>.Some some ? await func(some.Value) : Option.None<TResult>();
    }
}
