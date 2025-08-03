using System;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Enables LINQ-style composition for <see cref="Result{T, TError}" /> types using <c>Select</c> and <c>SelectMany</c>.
/// </summary>
public static class ResultLinqExtensions
{
    /// <summary>
    ///     Binds and projects over a <see cref="Result{TSource, TError}" /> using LINQ-style composition.
    /// </summary>
    /// <typeparam name="TSource">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TCollection">The intermediate value returned by the binding function.</typeparam>
    /// <typeparam name="TResult">The final projected result value.</typeparam>
    /// <param name="source">The initial result to bind.</param>
    /// <param name="bind">A function that returns a result from the source value.</param>
    /// <param name="project">A projection that combines the source and bound values.</param>
    /// <returns>
    ///     A <see cref="Result{TResult, TError}" /> containing the projected success value,
    ///     or the first encountered error.
    /// </returns>
    public static Result<TResult, TError> SelectMany<TSource, TError, TCollection, TResult>(
        this Result<TSource, TError>               source,
        Func<TSource, Result<TCollection, TError>> bind,
        Func<TSource, TCollection, TResult>        project) =>
        source.Match(
                     ok => bind(ok).Match<Result<TResult, TError>>(
                                                                   inner => new Result<TResult, TError>.Ok(project(ok, inner)),
                                                                   err => new Result<TResult, TError>.Error(err)
                                                                  ),
                     err => new Result<TResult, TError>.Error(err)
                    );

    /// <summary>
    ///     Binds a result-producing function to a <see cref="Result{T, TError}" />, enabling LINQ-style monadic chaining.
    /// </summary>
    /// <typeparam name="T">The original type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the bound result’s success value.</typeparam>
    /// <param name="result">The original result to bind.</param>
    /// <param name="binder">A function returning a new result from the success value.</param>
    /// <returns>
    ///     The bound result if successful, or the original error.
    /// </returns>
    public static Result<TNew, TError> SelectMany<T, TError, TNew>(
        this Result<T, TError>        result,
        Func<T, Result<TNew, TError>> binder) =>
        result.Bind(binder);

    /// <summary>
    ///     Transforms the success value of a <see cref="Result{T, TError}" /> using a mapping function.
    /// </summary>
    /// <typeparam name="T">The original type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the transformed value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <param name="selector">A function that transforms the success value.</param>
    /// <returns>
    ///     A result containing the mapped value if successful, or the original error.
    /// </returns>
    public static Result<TNew, TError> Select<T, TError, TNew>(
        this Result<T, TError> result,
        Func<T, TNew>          selector) =>
        result.Map(selector);
}
