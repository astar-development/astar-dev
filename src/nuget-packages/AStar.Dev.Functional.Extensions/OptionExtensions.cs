using System;
using System.Collections.Generic;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Functional helpers and utilities for working with <see cref="Option{T}" />.
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    ///     Attempts to extract the value from an <see cref="Option{T}" />.
    /// </summary>
    public static bool TryGetValue<T>(this Option<T> option, out T value)
    {
        if (option is Option<T>.Some some)
        {
            value = some.Value;

            return true;
        }

        value = default!;

        return false;
    }

    /// <summary>
    ///     Converts a value to an <see cref="Option{T}" />, treating default/null as <c>None</c>.
    /// </summary>
    public static Option<T> ToOption<T>(this T value) =>
        EqualityComparer<T>.Default.Equals(value, default!)
            ? Option.None<T>()
            : new Option<T>.Some(value);

    /// <summary>
    ///     Converts a value to an <see cref="Option{T}" /> if it satisfies the predicate.
    /// </summary>
    public static Option<T> ToOption<T>(this T value, Func<T, bool> predicate) =>
        predicate(value)
            ? new Option<T>.Some(value)
            : Option.None<T>();

    /// <summary>
    ///     Converts a nullable value type to an <see cref="Option{T}" />.
    /// </summary>
    public static Option<T> ToOption<T>(this T? nullable) where T : struct =>
        nullable.HasValue
            ? new Option<T>.Some(nullable.Value)
            : Option.None<T>();

    /// <summary>
    ///     Transforms the value inside an <see cref="Option{T}" /> if present.
    /// </summary>
    public static Option<TResult> Map<T, TResult>(this Option<T> option, Func<T, TResult> map) =>
        option.Match(
                     some => new Option<TResult>.Some(map(some)),
                     Option.None<TResult>);

    /// <summary>
    ///     Chains another <see cref="Option{T}" />-producing function.
    /// </summary>
    public static Option<TResult> Bind<T, TResult>(this Option<T> option, Func<T, Option<TResult>> bind) => option.Match(bind, Option.None<TResult>);

    /// <summary>
    ///     Converts an <see cref="Option{T}" /> to a <see cref="Result{T, TError}" />.
    /// </summary>
    public static Result<T, TError> ToResult<T, TError>(this Option<T> option, Func<TError> errorFactory) =>
        option.Match<Result<T, TError>>(
                                        some => new Result<T, TError>.Ok(some),
                                        () => new Result<T, TError>.Error(errorFactory()));

    /// <summary>
    ///     Determines whether the option contains a value.
    /// </summary>
    public static bool IsSome<T>(this Option<T> option) => option is Option<T>.Some;

    /// <summary>
    ///     Determines whether the option is empty.
    /// </summary>
    public static bool IsNone<T>(this Option<T> option) => option is Option<T>.None;

    /// <summary>
    ///     Converts an <see cref="Option{T}" /> to a nullable type.
    /// </summary>
    public static T? ToNullable<T>(this Option<T> option) where T : struct => option is Option<T>.Some some ? some.Value : null;

    /// <summary>
    ///     Converts an <see cref="Option{T}" /> to a single-element enumerable or an empty sequence.
    /// </summary>
    public static IEnumerable<T> ToEnumerable<T>(this Option<T> option) => option is Option<T>.Some some ? [ some.Value ] : [];

    /// <summary>
    ///     Gets the value of the option or returns a fallback value.
    /// </summary>
    public static T OrElse<T>(this Option<T> option, T fallback) => option is Option<T>.Some some ? some.Value : fallback;

    /// <summary>
    ///     Gets the value of the option or throws an exception if absent.
    /// </summary>
    public static T OrThrow<T>(this Option<T> option, Exception? ex = null) => option is Option<T>.Some some ? some.Value : throw ex ?? new InvalidOperationException("No value present");

    /// <summary>
    ///     Enables deconstruction of an option into a boolean and value pair.
    /// </summary>
    /// <param name="option"></param>
    /// <param name="isSome"></param>
    /// <param name="value"></param>
    /// <typeparam name="T"></typeparam>
    public static void Deconstruct<T>(this Option<T> option, out bool isSome, out T? value)
    {
        isSome = option is Option<T>.Some;
        value  = isSome ? ((Option<T>.Some)option).Value : default;
    }
}
