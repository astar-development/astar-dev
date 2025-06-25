// namespace AStar.Dev.Functional.Extensions;
//
// /// <summary>
// ///     The <see cref="ResultExtensions" /> class containing any available extensions
// /// </summary>
// public static class ResultExtensions
// {
//     /// <summary>
//     /// </summary>
//     /// <param name="func"></param>
//     /// <typeparam name="TResult"></typeparam>
//     /// <typeparam name="TError"></typeparam>
//     /// <returns></returns>
//     public static Result<TResult, string> TrySafe<TResult, TError>(this Func<TResult> func)
//     {
//         try
//         {
//             return func();
//         }
//         catch(Exception exception)
//         {
//             return Result<TResult, string>.Failure(exception.Message);
//         }
//     }
//
//     /// <summary>
//     ///
//     /// </summary>
//     /// <param name="result"></param>
//     /// <param name="func"></param>
//     /// <typeparam name="TResult"></typeparam>
//     /// <returns></returns>
//     public static Result<TResult, Error> TrySafe<TResult>(this Result<TResult, Error> result, Action<TResult> func)
//     {
//         try
//         {
//             func(result.Value);
//         }
//         catch (TaskCanceledException) { }
//         catch(Exception exception)
//         {
//             return Result<TResult, Error>.Fail(exception.Message);
//         }
//
//         return result;
//     }
//
//     /// <summary>
//     ///
//     /// </summary>
//     /// <param name="result"></param>
//     /// <param name="action"></param>
//     /// <typeparam name="TResult"></typeparam>
//     /// <typeparam name="TError"></typeparam>
//     /// <returns></returns>
//     public static Result<TResult, Error> OnSuccess<TResult, TError>(this Result<TResult, Error> result, Action<TResult> action)
//     {
//         if (result.IsFailure)
//         {
//             return Result<TResult, Error>.Fail(result.Error);
//         }
//
//         action(result.Value);
//         return result;
//
//     }
// }


