using System;
using System.Threading;
using System.Threading.Tasks;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Provides LINQ-style asynchronous binding for <see cref="Result{T, TError}" /> wrapped in <see cref="ValueTask" />.
/// </summary>
public static class ResultAsyncLinqExtensions
{
    /// <summary>
    ///     Asynchronously binds and projects over a <see cref="Result{T, TError}" /> using LINQ-style composition.
    /// </summary>
    /// <typeparam name="TSource">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TCollection">The type of the intermediate value returned by the binding function.</typeparam>
    /// <typeparam name="TResult">The final projected result type.</typeparam>
    /// <param name="source">
    ///     An asynchronous <see cref="Result{TSource, TError}" /> to be bound and projected.
    /// </param>
    /// <param name="bind">
    ///     A function that returns an asynchronous <see cref="Result{TCollection, TError}" /> from the source value.
    /// </param>
    /// <param name="project">
    ///     A projection function that combines the source and collection values into a final result.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token used to cancel the operation before completion.
    /// </param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> containing either a projected <c>Ok</c> value or an <c>Error</c> from any failure.
    /// </returns>
    public static async ValueTask<Result<TResult, TError>> SelectMany<TSource, TError, TCollection, TResult>(
        this ValueTask<Result<TSource, TError>>               source,
        Func<TSource, ValueTask<Result<TCollection, TError>>> bind,
        Func<TSource, TCollection, TResult>                   project,
        CancellationToken                                     cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await source;

        return result is Result<TSource, TError>.Ok ok
                   ? await bind(ok.Value).MapAsync(
                                                   inner => project(ok.Value, inner),
                                                   cancellationToken)
                   : new Result<TResult, TError>.Error(((Result<TSource, TError>.Error)result).Reason);
    }
}
