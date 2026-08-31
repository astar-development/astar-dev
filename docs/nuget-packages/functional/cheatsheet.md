# AStarDev.FunctionalParadigm Cheatsheet

Namespace: `AStar.Dev.FunctionalParadigm` (composition helpers: `AStar.Dev.FunctionalParadigm.Composition`, separate `using`).

Every monadic type below has sync + async (`Task`/`ValueTask`) overloads of `Map`/`Bind`/`Tap`/`Match` unless noted. Full write-up: [astardev-functionalparadigm-usage.md](astardev-functionalparadigm-usage.md).

## Types at a glance

| Type | Cases | Create |
| ---- | ----- | ------ |
| `Option<T>` | `Some`, `None` | `Option.Some(v)`, `Option.None<T>()`, implicit `T` (null → None) |
| `Result<TResult, TError>` | `Ok`, `Fail` | `Result.Success<TResult,TError>(v)`, `Result.Failure<TResult,TError>(e)`, implicit `TResult`/`TError` |
| `Exceptional<T>` | `Success`, `Failure` | `Exceptional.Success(v)`, `Exceptional.Failure(ex)`, implicit `T`/`Exception`, or `Try.Run`/`Try.RunAsync` |
| `Validation<T>` | `Valid`, `Invalid` | `Validation.Valid(v)`, `Validation.Invalid<T>(ValidationError)`, `Validation.Invalid<T>(IReadOnlyList<ValidationError>)` |
| `UnitFp` | — | `UnitFp.Instance` (no-value success marker, e.g. `Result<UnitFp, TError>`) |
| `ErrorResponse` | — | `new ErrorResponse(string Message)` — default `TError` |
| `ValidationError` | — | `new ValidationError(string Property, string Message)` or `ValidationErrorFactory.Create(property, message)` |

## `Option<T>`

| Member | Signature |
| ---- | ----- |
| Match | `Match<TResult>(Func<T,TResult> onSome, Func<TResult> onNone)` |
| TryGetValue | `TryGetValue(out T value) : bool` |
| Map | `Map<TResult>(Func<T,TResult>)` |
| Bind | `Bind<TResult>(Func<T,Option<TResult>>)` |
| Filter | `Filter(Func<T,bool>)` |
| Tap | `Tap(Action<T>)` — returns original `Option<T>` |
| MapOrDefault | `MapOrDefault<TResult>(Func<T,TResult> map, TResult defaultValue)` |
| MapOrElse | `MapOrElse<TResult>(Func<T,TResult> map, Func<TResult> defaultFactory)` |
| ToOption | `T.ToOption()`, `T.ToOption(Func<T,bool> predicate)`, `T?.ToOption()` (struct) |
| ToNullable | `ToNullable() : T?` (struct) |
| ToEnumerable | `ToEnumerable() : IEnumerable<T>` |
| ToResult | `ToResult<TError>(Func<TError> errorFactory) : Result<T,TError>` |
| Values | `IEnumerable<Option<T>>.Values() : IEnumerable<T>` — drops Nones |
| Choose | `IEnumerable<T>.Choose(Func<T,bool>) : IEnumerable<Option<T>>` or `.Choose(Func<T,Option<TResult>>) : IEnumerable<TResult>` |
| FirstOrNone | `IEnumerable<T>.FirstOrNone()`, `.FirstOrNone(Func<T,bool>)` — in `LinqExtensions` |
| FirstOrNoneAsync | `IAsyncEnumerable<T>.FirstOrNoneAsync(...)` |
| LINQ syntax | `Select`/`SelectMany` support `from x in option select ...` |
| Async | `MapAsync`, `BindAsync`, `TapAsync`, `MatchAsync`, `ToResultAsync` — on `Option<T>` and `Task<Option<T>>` |

## `Result<TResult, TError>`

| Member | Signature |
| ---- | ----- |
| Match | `Match<TOut>(Func<TResult,TOut> onSuccess, Func<TError,TOut> onFailure)` |
| Map | `Map<TMapped>(Func<TResult,TMapped>)` |
| Bind | `Bind<TMapped>(Func<TResult,Result<TMapped,TError>>)` |
| Tap | `Tap(Action<TResult> onSuccess, Action<TError>? onFailure = null)` |
| TapError | `TapError(Action<TError> onFailure)` |
| Ensure | `Ensure(Action finallyAction)` — runs regardless of outcome |
| OrElseAsync | `Task<Result<..>>.OrElseAsync(Func<TError,Task<Result<TResult,TError>>> fallback)` |
| Retry | `RetryExtensions.RetryOnceAsync(Func<Task<Result<TResult,TError>>> operation, Func<Task> onRetry)` — 1 retry, static method (not extension) |
| Async | `MapAsync`, `BindAsync`, `TapAsync`, `MatchAsync`, `EnsureAsync` — `Task`/`ValueTask` overloads |

## `Exceptional<T>`

| Member | Signature |
| ------ | --------- |
| Try.Run | `Try.Run(Func<T> operation)`, `Try.Run(op, CancellationToken)` — catches thrown exceptions into `Failure<T>` |
| Try.RunAsync | `Try.RunAsync(Func<Task<T>> operation)`, `Try.RunAsync(op, CancellationToken)` |
| — | `OperationCanceledException`/`TaskCanceledException` always rethrow, never captured |
| Match | `Match<TOut>(Func<T,TOut> onSuccess, Func<Exception,TOut> onFailure)` |
| Map / Bind | `Map<TResult>(Func<T,TResult>)`, `Bind<TResult>(Func<T,Exceptional<TResult>>)` |
| Tap / TapError | `Tap(Action<T> onSuccess, Action<Exception>? onFailure = null)`, `TapError(Action<Exception>)` |
| Ensure | `Ensure(Action finallyAction)` (sync); `EnsureAsync(this Task<Exceptional<T>>, Action<T> finallyAction)` |
| ToResult | `ToResult<TError>(Func<Exception,TError> mapError) : Result<T,TError>` |
| Async | `MapAsync`, `BindAsync`, `TapAsync`, `TapErrorAsync`, `MatchAsync`, `ToResultAsync` |

## `Validation<T>`

| Member | Signature |
| ---- | ----- |
| Match | `Match<TOut>(Func<T,TOut> onValid, Func<IReadOnlyList<ValidationError>,TOut> onInvalid)` |
| TryGetValue / TryGetErrors | `TryGetValue(out T)`, `TryGetErrors(out IReadOnlyList<ValidationError>)` |
| Apply | `Validation<Func<T,TResult>>.Apply(Validation<T>) : Validation<TResult>` — applicative, **curried** (one arg at a time), accumulates errors from both sides |
| Combine | `IEnumerable<Validation<T>>.Combine() : Validation<IReadOnlyList<T>>` — accumulates errors in order |
| ToResult | `ToResult<TError>(Func<IReadOnlyList<ValidationError>,TError> mapErrors) : Result<T,TError>` |

No async variants — validation is expected to be synchronous.

## Composition (`AStar.Dev.FunctionalParadigm.Composition`)

Requires its own `using` — its `Tap<T>` deliberately collides with the `Tap` overloads above, so it's opt-in.

| Member | Signature |
| ---- | ----- |
| Pipe | `TIn.Pipe<TOut>(Func<TIn,TOut>) : TOut` |
| PipeAsync | `TIn.PipeAsync<TOut>(Func<TIn,Task<TOut>>) : Task<TOut>` |
| Tap | `T.Tap(Action<T> sideEffect) : T` — generic, any type |
| Compose | `Func<TIn,TMid>.Compose<TOut>(Func<TMid,TOut>) : Func<TIn,TOut>` |

## Quick recipes

```csharp
using AStar.Dev.FunctionalParadigm;

// Option: parse-or-none, then fall through to a Result
Result<int, ErrorResponse> ParseAge(string raw) =>
    raw.ToOption()
       .Bind(s => int.TryParse(s, out var n) ? Option.Some(n) : Option.None<int>())
       .ToResult(() => new ErrorResponse("Invalid age"));

// Result: chain + tap + fallback error
Result<int, ErrorResponse> Divide(int a, int b) =>
    b == 0 ? new ErrorResponse("div by zero") : Result.Success<int, ErrorResponse>(a / b);

var r = Divide(10, 2)
    .Bind(v => Divide(v, 5))
    .Tap(v => Console.WriteLine(v), e => Console.WriteLine(e.Message));

// Exceptional: catch a throwing call, join into Result
Result<int, ErrorResponse> parsed = Try.Run(() => int.Parse("42"))
    .ToResult(ex => new ErrorResponse(ex.Message));

// Validation: accumulate field errors (curried applicative)
public record Person(string Name, int Age);

Validation<Person> ValidatePerson(string name, int age) =>
    Validation.Valid<Func<string, Func<int, Person>>>(n => a => new Person(n, a))
        .Apply(ValidateName(name))
        .Apply(ValidateAge(age));
```

```csharp
using AStar.Dev.FunctionalParadigm.Composition;

var text = 5.Pipe(n => n * 2).Tap(n => Console.WriteLine(n)).Pipe(n => n.ToString());
```

## Gotchas (agents & humans)

- `Composition.Tap` and `Option`/`Result`/`Exceptional`'s `Tap` collide on overload resolution if both namespaces are `using`-imported for a single-argument call — don't mix them in the same file without qualifying.
- `Apply` on `Validation<T>` is curried: a 2-arg constructor needs `Func<T1, Func<T2, TResult>>`, not `Func<T1, T2, TResult>`.
- `Try.Run`/`Try.RunAsync` never capture `OperationCanceledException` — it always propagates.
- `Option<T>.Some` throws `ArgumentNullException` if constructed directly with a null value — use `Option.Some`/implicit conversion/`ToOption()` instead of `new Option<T>.Some(...)` with a possibly-null value.
- `Validation<T>` has no async surface — validate synchronously, convert to `Result`/`Exceptional` first if you need async.
