namespace Valuable.Fallible.Extensions;

public static class Variance
{
    public static Fallible<TCovariant> ToCovariant<T, TCovariant>(this Fallible<T> fallible)
        where T : TCovariant
    {
        var (value, error) = fallible;
        
        return error ? error : value;
    }

    public static Fallible<TContravariant> ToContravariant<T, TContravariant>(this Fallible<T> fallible)
        where TContravariant : T
    {
        var (value, error) = fallible;

        return error ? error : (TContravariant) value!;
    }
}