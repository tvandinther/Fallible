namespace FallibleTypes.Extensions.Continuation;

public static partial class FallibleExtensions
{
    // public static Fallible<TResult> OnFail<TResult>(this Fallible<TResult> fallible, Func<TResult> onFail)
    // {
    //     return fallible.Error ? onFail() : fallible;
    // }
    
    public static Fallible<TResult> OnFail<TResult>(this Fallible<TResult> fallible, Func<Fallible<TResult>> onFail)
    {
        return fallible.Error ? onFail() : fallible;
    }
    
    public static Fallible<TResult> OnFail<TResult>(this Fallible<TResult> fallible, Func<Error, TResult> onFail)
    {
        return OnFail(fallible, () => onFail(fallible.Error));
    }

    public static Fallible<TResult> OnFail<TResult>(this Fallible<TResult> fallible, Func<Error, Fallible<TResult>> onFail)
    {
        return OnFail(fallible, () => onFail(fallible.Error));
    }
}