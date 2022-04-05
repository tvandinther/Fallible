namespace FallibleTypes;

public static partial class Fallible
{
    public static Fallible<TIn> Recover<TIn>(this Fallible<TIn> fallible, Func<Fallible<TIn>> onFail)
    {
        return fallible.Error ? onFail() : fallible;
    }

    public static Fallible<TIn> Recover<TIn>(this Fallible<TIn> fallible, Func<Error, Fallible<TIn>> onFail)
    {
        return Recover(fallible, () => onFail(fallible.Error));
    }
}