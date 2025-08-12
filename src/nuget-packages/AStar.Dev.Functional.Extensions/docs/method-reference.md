# 📋 Method Reference

This document provides a comprehensive reference for all methods available in the type. `Result<TSuccess, TError>`

## 🔍 Pattern Matching

### Match

``` csharp
public TResult Match<TResult>(
    Func<TSuccess, TResult> onSuccess,
    Func<TError, TResult> onFailure)
```

Executes one of the provided functions based on whether the result is successful or not.
**Example:**

``` csharp
string message = result.Match(
    book => $"Found: {book.Title}",
    error => $"Error: {error}"
);
```

### MatchAsync

``` csharp
public Task<TResult> MatchAsync<TResult>(
    Func<TSuccess, Task<TResult>> onSuccess,
    Func<TError, TResult> onFailure)
```

Async version of Match where success handler is asynchronous.

``` csharp
public Task<TResult> MatchAsync<TResult>(
    Func<TSuccess, TResult> onSuccess,
    Func<TError, Task<TResult>> onFailure)
```

Async version of Match where failure handler is asynchronous.

``` csharp
public Task<TResult> MatchAsync<TResult>(
    Func<TSuccess, Task<TResult>> onSuccess,
    Func<TError, Task<TResult>> onFailure)
```

Async version of Match where both handlers are asynchronous.
**Example:**

``` csharp
string message = await result.MatchAsync(
    async book => await FormatBookAsync(book),
    error => $"Error: {error}"
);
```

## 🔄 Transformations

### Map

``` csharp
public Result<TNew, TError> Map<TNew>(
    Func<TSuccess, TNew> map)
```

Transforms the success value using the provided function.
**Example:**

``` csharp
Result<BookTitle, string> titleResult = bookResult.Map(book => book.Title);
```

### MapAsync

``` csharp
public Task<Result<TNew, TError>> MapAsync<TNew>(
    Func<TSuccess, Task<TNew>> mapAsync)
```

Async version of Map where the mapping function is asynchronous.
**Example:**

``` csharp
Result<BookDetails, string> detailsResult = await bookResult.MapAsync(
    async book => await GetBookDetailsAsync(book)
);
```

### MapFailure

``` csharp
public Result<TSuccess, TNewError> MapFailure<TNewError>(
    Func<TError, TNewError> mapError)
```

Transforms the error value using the provided function.
**Example:**

``` csharp
Result<Book, UserFriendlyError> userFriendlyResult = 
    bookResult.MapFailure(error => new UserFriendlyError($"Could not find book: {error}"));
```

### MapFailureAsync

``` csharp
public Task<Result<TSuccess, TNewError>> MapFailureAsync<TNewError>(
    Func<TError, Task<TNewError>> mapErrorAsync)
```

Async version of MapFailure where the mapping function is asynchronous.
**Example:**

``` csharp
Result<Book, UserFriendlyError> userFriendlyResult = 
    await bookResult.MapFailureAsync(async error => await TranslateErrorAsync(error));
```

## ⛓️ Composition

### Bind

``` csharp
public Result<TNew, TError> Bind<TNew>(
    Func<TSuccess, Result<TNew, TError>> bind)
```

Chains the result to another result-producing function.
**Example:**

``` csharp
Result<OrderConfirmation, string> orderResult = 
    bookResult.Bind(book => PlaceOrder(book, quantity));
```

### BindAsync

``` csharp
public Task<Result<TNew, TError>> BindAsync<TNew>(
    Func<TSuccess, Task<Result<TNew, TError>>> bindAsync)
```

Async version of Bind where the binding function is asynchronous.
**Example:**

``` csharp
Result<OrderConfirmation, string> orderResult = 
    await bookResult.BindAsync(book => PlaceOrderAsync(book, quantity));
```

## 📊 Side Effects

### Tap

``` csharp
public Result<TSuccess, TError> Tap(
    Action<TSuccess> action)
```

Performs a side effect on the success value and returns the original result.
**Example:**

``` csharp
bookResult.Tap(book => Console.WriteLine($"Found book: {book.Title}"));
```

### TapAsync

``` csharp
public Task<Result<TSuccess, TError>> TapAsync(
    Func<TSuccess, Task> actionAsync)
```

Async version of Tap where the action is asynchronous.
**Example:**

``` csharp
await bookResult.TapAsync(async book => await LogBookAccessAsync(book));
```

### TapError

``` csharp
public Result<TSuccess, TError> TapError(
    Action<TError> action)
```

Performs a side effect on the error value and returns the original result.
**Example:**

``` csharp
bookResult.TapError(error => Console.WriteLine($"Error finding book: {error}"));
```

### TapErrorAsync

``` csharp
public Task<Result<TSuccess, TError>> TapErrorAsync(
    Func<TError, Task> actionAsync)
```

Async version of TapError where the action is asynchronous.
**Example:**

``` csharp
await bookResult.TapErrorAsync(async error => await LogErrorAsync(error));
```

## 🧰 Extension Methods

Here are some useful extension methods you might want to implement:

### OnSuccess

``` csharp
public static Result<TSuccess, TError> OnSuccess<TSuccess, TError>(
    this Result<TSuccess, TError> result,
    Action<TSuccess> action)
{
    return result.Tap(action);
}
```

### OnFailure

``` csharp
public static Result<TSuccess, TError> OnFailure<TSuccess, TError>(
    this Result<TSuccess, TError> result,
    Action<TError> action)
{
    return result.TapError(action);
}
```

### ToResult

``` csharp
public static Result<T, Exception> ToResult<T>(this Task<T> task)
{
    try
    {
        return new Result<T, Exception>.Ok(task.Result);
    }
    catch (Exception ex)
    {
        return new Result<T, Exception>.Error(ex);
    }
}
```

### ToResultAsync

``` csharp
public static async Task<Result<T, Exception>> ToResultAsync<T>(this Task<T> task)
{
    try
    {
        T result = await task;
        return new Result<T, Exception>.Ok(result);
    }
    catch (Exception ex)
    {
        return new Result<T, Exception>.Error(ex);
    }
}
```

### Unwrap

``` csharp
public static TSuccess Unwrap<TSuccess, TError>(
    this Result<TSuccess, TError> result,
    TSuccess defaultValue = default)
{
    return result.Match(
        success => success,
        _ => defaultValue
    );
}
```
