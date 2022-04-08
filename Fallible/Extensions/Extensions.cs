namespace FallibleTypes.Extensions;

public static class Extensions
{
    public static Fallible<T> ToFallible<T>(this T value)
    {
        return value;
    }
}