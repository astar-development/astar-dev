# 🎯 Option<T> Functional Cheat Sheet

## 🧩 Option Overview

Represents a value that might exist (`Some`) or not (`None`), avoiding nulls and enabling functional composition.

```csharp
Option<int> maybeNumber = Option.Some(42);
Option<string> emptyName = Option.None<string>();
```

---

## 🏗 Construction

| Syntax                      | Description                             |
|-----------------------------|-----------------------------------------|
| `Option.Some(value)`        | Wraps a non-null value as `Some`        |
| `Option.None<T>()`          | Creates a `None` of type `T`            |
| `value.ToOption()`          | Converts value or default to Option     |
| `value.ToOption(predicate)` | Converts only if predicate returns true |
| `nullable.ToOption()`       | Converts nullable struct to Option      |

---

## 🧪 Pattern Matching

```csharp
option.Match(
    some => $"Value: {some}",
    ()   => "No value"
);
```

Or via deconstruction:

```csharp
var (isSome, value) = option;
```

Or with TryGet:

```csharp
if (option.TryGetValue(out var value)) { /* use value */ }
```

---

## 🔧 Transformation

| Method              | Description                                 |
|---------------------|---------------------------------------------|
| `Map(func)`         | Transforms value inside Some                |
| `Bind(func)`        | Chains function that returns another Option |
| `ToResult(errorFn)` | Converts Option to `Result<T, TError>`      |
| `ToNullable()`      | Converts to nullable (structs only)         |
| `ToEnumerable()`    | Converts to `IEnumerable<T>`                |

---

## 🪄 LINQ Support

```csharp
var result =
    from name in Option.Some("Jason")
    from greeting in Option.Some($"Hello, {name}")
    select greeting;
```

Via `Select`, `SelectMany`, or `SelectAwait` (async LINQ)

---

## 🔁 Async Support

| Method                       | Description                                   |
|------------------------------|-----------------------------------------------|
| `MapAsync(func)`             | Awaits and maps value                         |
| `BindAsync(func)`            | Awaits and chains async Option-returning func |
| `MatchAsync(onSome, onNone)` | Async pattern match                           |
| `SelectAwait(func)`          | LINQ-friendly async projection                |

---

## 🧯 Fallbacks and Conversions

```csharp
option.OrElse("fallback");      // returns value or fallback
option.OrThrow();               // throws if None
option.IsSome();                // true if Some
option.IsNone();                // true if None
```

---

## 🐛 Debugging & Output

```csharp
option.ToString(); // Outputs "Some(value)" or "None"
```

---

