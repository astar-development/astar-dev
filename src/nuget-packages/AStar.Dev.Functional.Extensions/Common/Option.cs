using System;
using System.Collections.Generic;
using System.Linq;

namespace AStar.Dev.Functional.Extensions.Common;

public class Option<T>
{
    private readonly T    _content;
    private readonly bool _hasValue;

    private Option(T content, bool hasValue) =>
        (_content, _hasValue) = (content, hasValue);

    public static Option<T> Some(T content) => new(content, true);
    public static Option<T> None() => new(default!, false);

    public T Get(T defaultValue) =>
        _hasValue ? _content : defaultValue;

    public Option<R> Map<R>(Func<T, R> map) =>
        _hasValue ? Option<R>.Some(map(_content)) : Option<R>.None();

    public Option<R> Bind<R>(Func<T, Option<R>> bind) =>
        _hasValue ? bind(_content) : Option<R>.None();

    public override string ToString() =>
        _hasValue ? _content?.ToString() ?? string.Empty : string.Empty;
}

public static class OptionExtensions
{
    public static Option<T> FirstOrNone<T>(this IEnumerable<T> items, Func<T, bool> predicate) =>
        items
            .Where(predicate)
            .Select(Option<T>.Some)
            .DefaultIfEmpty(Option<T>.None())
            .First();

    public static Option<R> Select<T, R>(this Option<T> obj, Func<T, R> map) => obj.Map(map);

    public static Option<T> Where<T>(this Option<T> obj, Func<T, bool> predicate) =>
        obj.Bind(content => predicate(content) ? obj : Option<T>.None());

    public static Option<TResult> SelectMany<T, R, TResult>(
        this Option<T> obj, Func<T, Option<R>> bind, Func<T, R, TResult> map) =>
        obj.Bind(original => bind(original).Map(result => map(original, result)));
}
