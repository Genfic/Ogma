namespace Utils.Extensions;

public static class FluentHelpers
{
    public static T If<T>(this T builder, bool condition, Func<T, T> func)
        => condition
            ? func(builder)
            : builder;

    public static T If<T>(this T builder, Func<bool> predicate, Func<T, T> func)
        => predicate()
            ? func(builder)
            : builder;

    public static T If<T>(this T builder, Func<T, bool> predicate, Func<T, T> func)
        => predicate(builder)
            ? func(builder)
            : builder;
}
