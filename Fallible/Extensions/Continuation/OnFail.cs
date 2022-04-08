namespace FallibleTypes.Extensions.Continuation;

public static partial class FallibleExtensions
{
    public static Fallible<TValue> OnFail<TValue>(this Fallible<TValue> fallible, Func<Fallible<TValue>> onFail)
    {
        return fallible.Error ? onFail() : fallible;
    }

    public static Fallible<TValue> OnFail<TValue>(this Fallible<TValue> fallible, Func<Error, Fallible<TValue>> onFail)
    {
        return OnFail(fallible, () => onFail(fallible.Error));
    }
}