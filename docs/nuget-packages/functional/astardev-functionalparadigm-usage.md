# AStarDev Functional Paradigm Usage

`AStarDev.FunctionalParadigm` is a small, dependency-free library that brings a handful of functional-programming staples to C#: `Option<T>` for optional values, `Result<TResult, TError>` for operations that can fail with a typed error, `Exceptional<T>` for wrapping code that throws, `Validation<T>` for accumulating multiple errors instead of stopping at the first one, and a couple of composition helpers for chaining plain functions together.

None of this is exotic — if you've used `Option`/`Maybe`, `Result`/`Either` or applicative validation in F#, Rust, Scala or any of the popular functional C# libraries, the shapes here will feel familiar. The goal of this post is to walk through what's actually in the package, with examples that compile against the real API.

Everything below lives in the `AStar.Dev.FunctionalParadigm` namespace, except the composition helpers, which live in `AStar.Dev.FunctionalParadigm.Composition`.

## `Option<T>`: modelling "maybe nothing"

`Option<T>` replaces `null` with an explicit type: a value is either `Some` (present) or `None` (absent). You create one with the `Option` factory class, or let the implicit conversion do it for you:

```csharp
using AStar.Dev.FunctionalParadigm;

Option<string> name = Option.Some("Ada");
Option<string> missing = Option.None<string>();

// The implicit conversion treats a null value as None.
Option<string> alsoAda = "Ada";
```

To get a value back out, pattern-match with `Match`:

```csharp
string greeting = name.Match(
    onSome: n => $"Hello, {n}!",
    onNone: () => "Hello, stranger!");
```

or use the `bool`-returning `TryGetValue`, which reads like the rest of the .NET standard library:

```csharp
if (name.TryGetValue(out var value))
{
    Console.WriteLine(value);
}
```

### Transforming and chaining

`Map` transforms the value if it's there; `Bind` does the same but for functions that themselves return an `Option<T>`, so you don't end up with an `Option<Option<T>>`:

```csharp
Option<int> length = name.Map(n => n.Length);

Option<int> parsedAge = "42"
    .ToOption()
    .Bind(s => int.TryParse(s, out var n) ? Option.Some(n) : Option.None<int>());
```

`Filter` turns a `Some` that fails a predicate into a `None`:

```csharp
Option<int> evenOnly = Option.Some(4).Filter(n => n % 2 == 0);
```

`Tap` runs a side effect (logging, say) without changing the option, and `MapOrDefault`/`MapOrElse` let you collapse straight to a plain value:

```csharp
name.Tap(n => Console.WriteLine($"Got a name: {n}"));

int nameLength = name.MapOrDefault(n => n.Length, defaultValue: 0);
int nameLengthOrComputed = name.MapOrElse(n => n.Length, defaultFactory: () => -1);
```

### Converting to and from other shapes

```csharp
int? nullableAge = Option.Some(30).ToNullable();
Option<int> fromNullable = nullableAge.ToOption();

IEnumerable<string> asSequence = name.ToEnumerable(); // zero or one element

Result<string, ErrorResponse> result = name.ToResult(() => new ErrorResponse("Name is required"));
```

### Working with sequences

`OptionExtensions` also has a few helpers for collections:

```csharp
var options = new[] { Option.Some(1), Option.None<int>(), Option.Some(3) };
IEnumerable<int> present = options.Values(); // 1, 3

IEnumerable<int> evens = new[] { 1, 2, 3, 4 }.Choose(n => n % 2 == 0 ? Option.Some(n) : Option.None<int>());
```

And `LinqExtensions` adds `FirstOrNone`, an `Option`-returning alternative to LINQ's `FirstOrDefault`:

```csharp
var numbers = new[] { 1, 2, 3, 4 };
Option<int> firstEven = numbers.FirstOrNone(n => n % 2 == 0);
Option<int> firstOverall = numbers.FirstOrNone();
```

There's an `IAsyncEnumerable<T>` version too — `FirstOrNoneAsync` — for async streams.

### LINQ query syntax

Because `OptionLinqExtensions` defines `Select` and `SelectMany`, you can use `Option<T>` directly in query-expression syntax:

```csharp
Option<int> a = Option.Some(2);
Option<int> b = Option.Some(3);

Option<int> sum =
    from x in a
    from y in b
    select x + y; // Some(5) — None if either a or b is None
```

### Async

Most of the operations above have `Task`-friendly counterparts — `MapAsync`, `BindAsync`, `TapAsync`, `MatchAsync`, `ToResultAsync` — that work whether you're holding an `Option<T>` and an async continuation, or a `Task<Option<T>>` you want to keep chaining off of:

```csharp
Task<Option<User>> FindUserAsync(int id) => userRepository.FindAsync(id);

Option<string> nameOption = await FindUserAsync(id).MapAsync(user => user.Name);

Option<User> user = await ParseUserIdAsync(raw).BindAsync(FindUserAsync);
```

## `Result<TResult, TError>`: typed success or failure

`Result<TResult, TError>` is for operations that can fail in a well-known way — think "parse this input" or "call this downstream service" — where you want the failure to be a first-class, typed value rather than an exception. It's an abstract `record` with two cases, `Ok<TResult, TError>` and `Fail<TResult, TError>`, created through the `Result` factory or via implicit conversion:

```csharp
using AStar.Dev.FunctionalParadigm;

Result<int, ErrorResponse> Divide(int a, int b) =>
    b == 0
        ? Result.Failure<int, ErrorResponse>(new ErrorResponse("Cannot divide by zero"))
        : Result.Success<int, ErrorResponse>(a / b);

// Implicit conversions work too.
Result<int, ErrorResponse> ok = 42;
Result<int, ErrorResponse> fail = new ErrorResponse("failed");
```

`Match` collapses a `Result` to a single value:

```csharp
Result<int, ErrorResponse> result = Divide(10, 2);

string message = result.Match(
    onSuccess: value => $"Result: {value}",
    onFailure: error => $"Error: {error.Message}");
```

### Transforming, chaining and side effects

`Map` transforms the success value; `Bind` chains another `Result`-returning operation, short-circuiting on failure:

```csharp
Result<string, ErrorResponse> asText = result.Map(value => value.ToString());

Result<int, ErrorResponse> chained = result.Bind(value => Divide(value, 5));
```

`Tap` and `TapError` run a side effect without changing the result, and `Ensure` runs a finalizer regardless of outcome:

```csharp
result
    .Tap(value => Console.WriteLine($"Got {value}"))
    .TapError(error => Console.WriteLine($"Failed: {error.Message}"))
    .Ensure(() => Console.WriteLine("Divide attempted"));
```

Every one of these has `Task<Result<..>>`/`ValueTask<Result<..>>` overloads (`MapAsync`, `BindAsync`, `TapAsync`, `MatchAsync`, `EnsureAsync`, `OrElseAsync`), so an async pipeline reads the same way as a synchronous one:

```csharp
Result<User, ErrorResponse> user = await FindUserAsync(id)
    .BindAsync(u => ValidateUserAsync(u))
    .TapAsync(u => Console.WriteLine($"Validated {u.Name}"));
```

### Retrying

`RetryExtensions.RetryOnceAsync` runs an operation, and if it fails, runs a callback (typically a delay) before trying exactly once more:

```csharp
Result<string, ErrorResponse> response = await RetryExtensions.RetryOnceAsync(
    operation: () => CallFlakyServiceAsync(),
    onRetry: () => Task.Delay(TimeSpan.FromSeconds(1)));
```

## `Exceptional<T>`: capturing exceptions as values

`Exceptional<T>` is `Result`'s sibling for code that throws instead of returning a typed error — third-party APIs, `int.Parse`, file I/O, and so on. `Try.Run`/`Try.RunAsync` invoke a delegate and catch whatever it throws into a `Failure<T>`:

```csharp
using AStar.Dev.FunctionalParadigm;

Exceptional<int> parsed = Try.Run(() => int.Parse("abc"));

Exceptional<string> content = await Try.RunAsync(() => httpClient.GetStringAsync(url));
```

`OperationCanceledException` (and `TaskCanceledException`) is deliberately never captured — it always rethrows, so cancellation still behaves the way callers expect.

The rest of the API mirrors `Result<TResult, TError>` — `Match`, `Map`, `Bind`, `Tap`, `TapError`, `Ensure`, and their async equivalents:

```csharp
string message = parsed.Match(
    onSuccess: value => $"Parsed {value}",
    onFailure: exception => $"Failed: {exception.Message}");
```

When you're ready to leave "might throw" territory and join it up with the rest of a `Result`-based pipeline, use `ToResult`:

```csharp
Result<int, ErrorResponse> result = parsed.ToResult(ex => new ErrorResponse(ex.Message));
```

## `Validation<T>`: accumulating errors instead of stopping at the first

`Result` and `Exceptional` both short-circuit — the first failure wins. `Validation<T>` is for the opposite case: validating a form, a DTO, a request, where you want to report *every* problem at once. It's a `Valid<T>` or an `Invalid<T>` carrying a list of `ValidationError`:

```csharp
using AStar.Dev.FunctionalParadigm;

Validation<string> ValidateName(string name) =>
    string.IsNullOrWhiteSpace(name)
        ? Validation.Invalid<string>(new ValidationError(nameof(name), "Name is required"))
        : Validation.Valid(name);

Validation<int> ValidateAge(int age) =>
    age is >= 0 and < 150
        ? Validation.Valid(age)
        : Validation.Invalid<int>(new ValidationError(nameof(age), "Age is out of range"));
```

### Combining validations applicatively

`Apply` lets you build up a validated object from individually-validated fields, accumulating errors from both sides when more than one field is invalid. Because `Apply` takes one argument at a time, the constructor function needs to be curried:

```csharp
public record Person(string Name, int Age);

Validation<Func<string, Func<int, Person>>> ctor =
    Validation.Valid<Func<string, Func<int, Person>>>(personName => personAge => new Person(personName, personAge));

Validation<Person> person = ctor
    .Apply(ValidateName("Ada"))
    .Apply(ValidateAge(200));

string summary = person.Match(
    onValid: p => $"Valid: {p.Name}, {p.Age}",
    onInvalid: errors => $"Invalid: {string.Join(", ", errors.Select(e => e.Message))}");
// "Invalid: Age is out of range"
```

If you just have a homogeneous sequence of validations and want them all to succeed together, `Combine` is simpler:

```csharp
var validations = new[] { ValidateName("Ada"), ValidateName("") };
Validation<IReadOnlyList<string>> combined = validations.Combine();
```

### Extracting values

Same shape as `Option` and `Validation`'s siblings — `TryGetValue`, `TryGetErrors`, and `ToResult` to join a validation pipeline into a `Result`-based one:

```csharp
if (person.TryGetValue(out var value))
{
    Console.WriteLine(value);
}

if (person.TryGetErrors(out var errors))
{
    Console.WriteLine(string.Join("; ", errors.Select(e => e.Message)));
}

Result<Person, ErrorResponse> result = person.ToResult(
    errs => new ErrorResponse(string.Join("; ", errs.Select(e => e.Message))));
```

## Composition helpers

`AStar.Dev.FunctionalParadigm.Composition` has three small helpers for chaining plain values and functions — useful outside of `Option`/`Result`/`Exceptional` pipelines, or for gluing them together. It's kept in its own namespace and opted into separately (`using AStar.Dev.FunctionalParadigm.Composition;`) because its generic `Tap<T>` would otherwise collide with the more specific `Tap` overloads on `Result`, `Option` and `Exceptional`.

```csharp
using AStar.Dev.FunctionalParadigm.Composition;

var output = 5
    .Pipe(n => n * 2)
    .Tap(n => Console.WriteLine($"Doubled: {n}"))
    .Pipe(n => n.ToString());
```

`Compose` glues two plain functions into one:

```csharp
Func<int, int> doubleIt = n => n * 2;
Func<int, string> describe = n => $"Value: {n}";
Func<int, string> doubleThenDescribe = doubleIt.Compose(describe);

string result = doubleThenDescribe(21); // "Value: 42"
```

There's also `PipeAsync` for piping a value through an async function.

## UnitFp and ErrorResponse

`UnitFp` is the functional "no meaningful value" type — useful as the success type of a `Result<UnitFp, TError>` for operations that succeed or fail but don't produce a value:

```csharp
Result<UnitFp, ErrorResponse> Save(Person person)
{
    // ... perform the side effect
    return Result.Success<UnitFp, ErrorResponse>(UnitFp.Instance);
}
```

`ErrorResponse` is a minimal error `record` with a single `Message` property, handy as a default `TError` when you don't need a richer error model of your own:

```csharp
var error = new ErrorResponse("Something went wrong");
Console.WriteLine(error.Message);
```

## Wrapping up

The four core types — `Option<T>`, `Result<TResult, TError>`, `Exceptional<T>` and `Validation<T>` — cover the same ground you'd reach for `null` checks, exceptions, and hand-rolled error-accumulation lists for. They compose the same way throughout (`Match`, `Map`, `Bind`, `Tap`), have full async coverage, and stay out of each other's way until you deliberately convert between them with `ToResult`/`ToOption`. That consistency is really the whole point: once the shape is familiar for one type, it's familiar for all of them.
