namespace FallibleTypes.Extensions;

public static class Map
{
    public static Fallible<T> AsFallible<T>(this T value)
    {
        return value;
    }
    
    public static Fallible<T> AsFallible<T>(this Fallible<T> value)
    {
        return value;
    }
    
    public static Func<Fallible<TOut>> AsFallible<TOut>(this Func<TOut> func)
    {
        return () => func();
    }
    
    public static Func<Fallible<TOut>> AsFallible<TOut>(this Func<Fallible<TOut>> func)
    {
        return func;
    }
    
    public static Func<Fallible<TIn>, Fallible<TOut>> AsFallible<TIn, TOut>(this Func<TIn, TOut> func)
    {
        return input => func(input.Value);
    }
    
    public static Func<Fallible<TIn>, Fallible<TOut>> AsFallible<TIn, TOut>(this Func<Fallible<TIn>, TOut> func)
    {
        return input => func(input.Value);
    }
    
    public static Func<Fallible<TIn>, Fallible<TOut>> AsFallible<TIn, TOut>(this Func<TIn, Fallible<TOut>> func)
    {
        return input => func(input.Value);
    }
    
    public static Func<Fallible<TIn>, Fallible<TOut>> AsFallible<TIn, TOut>(this Func<Fallible<TIn>, Fallible<TOut>> func)
    {
        return func;
    }
}