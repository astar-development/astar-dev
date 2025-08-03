using System;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Provides functional operations for transforming and composing <see cref="Result{T, TError}" />.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    ///     Transforms the success value of a <see cref="Result{T, TError}" /> using the specified mapping function.
    /// </summary>
    /// <typeparam name="T">The original type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the transformed success value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="map">A function that maps the original value to a new value.</param>
    /// <returns>
    ///     A new <see cref="Result{TNew, TError}" /> containing the mapped success value if present,
    ///     or the original error if unsuccessful.
    /// </returns>
    public static Result<TNew, TError> Map<T, TError, TNew>(
        this Result<T, TError> result,
        Func<T, TNew>          map) =>
        result.Match<Result<TNew, TError>>(
                                           ok => new Result<TNew, TError>.Ok(map(ok)),
                                           err => new Result<TNew, TError>.Error(err)
                                          );

    /// <summary>
    ///     Chains the current result to another <see cref="Result{TNew, TError}" />-producing function,
    ///     allowing for functional composition across result types.
    /// </summary>
    /// <typeparam name="T">The original type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the new result's success value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="bind">A function that returns a new <see cref="Result{TNew, TError}" />.</param>
    /// <returns>
    ///     The result of the binding function if the original was successful; otherwise, the original error.
    /// </returns>
    public static Result<TNew, TError> Bind<T, TError, TNew>(
        this Result<T, TError>        result,
        Func<T, Result<TNew, TError>> bind) =>
        result.Match(
                     bind,
                     err => new Result<TNew, TError>.Error(err)
                    );

    /// <summary>
    ///     Executes a side-effect action on the success value of a <see cref="Result{T, TError}" />,
    ///     returning the original result unchanged.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <param name="result">The result to inspect.</param>
    /// <param name="action">An action to invoke if the result is successful.</param>
    /// <returns>
    ///     The original <see cref="Result{T, TError}" /> instance, unchanged.
    /// </returns>
    public static Result<T, TError> Tap<T, TError>(
        this Result<T, TError> result,
        Action<T>              action)
    {
        if (result is Result<T, TError>.Ok ok)
        {
            action(ok.Value);
        }

        return result;
    }
}
