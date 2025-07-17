using System;
using System.Threading;
using System.Threading.Tasks;

namespace AStar.Dev.Functional.Extensions;

/// <summary>
///     Provides asynchronous functional operations for <see cref="Result{T, TError}" /> wrapped in <see cref="ValueTask" />.
/// </summary>
public static class ResultAsyncExtensions
{
    /// <summary>
    ///     Asynchronously transforms the success value of a <see cref="Result{T, TError}" /> if present.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the transformed success value.</typeparam>
    /// <param name="task">A <see cref="ValueTask{Result}" /> that wraps the result to transform.</param>
    /// <param name="map">A mapping function applied to the success value.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> containing either a transformed <c>Ok</c> result or the original <c>Error</c>.
    /// </returns>
    public static async ValueTask<Result<TNew, TError>> MapAsync<T, TError, TNew>(
        this ValueTask<Result<T, TError>> task,
        Func<T, TNew>                     map,
        CancellationToken                 cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await task;

        return result.Map(map);
    }

    /// <summary>
    ///     Asynchronously chains a function that returns another <see cref="Result{T, TError}" /> if the current result is successful.
    /// </summary>
    /// <typeparam name="T">The type of the original success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TNew">The type of the success value returned by the binding function.</typeparam>
    /// <param name="task">A <see cref="ValueTask{Result}" /> containing the result to bind.</param>
    /// <param name="bind">A function that returns a new asynchronous result.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask{Result}" /> containing the bound result if successful, or the original <c>Error</c>.
    /// </returns>
    public static async ValueTask<Result<TNew, TError>> BindAsync<T, TError, TNew>(
        this ValueTask<Result<T, TError>>        task,
        Func<T, ValueTask<Result<TNew, TError>>> bind,
        CancellationToken                        cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await task;

        return result is Result<T, TError>.Ok ok
                   ? await bind(ok.Value)
                   : new Result<TNew, TError>.Error(((Result<T, TError>.Error)result).Reason);
    }

    /// <summary>
    ///     Asynchronously matches on a <see cref="Result{T, TError}" />, executing the appropriate function depending on success or error.
    /// </summary>
    /// <typeparam name="T">The type of the success value.</typeparam>
    /// <typeparam name="TError">The type of the error value.</typeparam>
    /// <typeparam name="TResult">The return type of the match operation.</typeparam>
    /// <param name="task">A <see cref="ValueTask{Result}" /> to evaluate.</param>
    /// <param name="onSuccess">A function invoked if the result is <c>Ok</c>.</param>
    /// <param name="onError">A function invoked if the result is <c>Error</c>.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask{TResult}" /> containing the return value of the appropriate match function.
    /// </returns>
    public static async ValueTask<TResult> MatchAsync<T, TError, TResult>(
        this ValueTask<Result<T, TError>> task,
        Func<T, TResult>                  onSuccess,
        Func<TError, TResult>             onError,
        CancellationToken                 cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var result = await task;

        return result.Match(onSuccess, onError);
    }
}
