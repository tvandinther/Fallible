using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;

namespace Fallible;

public readonly record struct Fallible<T> : IStructuralEquatable, ITuple
{
    public T Value { get; }
    public Error Error { get; }

    private Fallible(T value, Error error)
    {
        Value = value;
        Error = error;
    }

    public static implicit operator Fallible<T>(Error error) => new(default!, error);
    public static implicit operator Fallible<T>(T value) => new(value, default!);
    public static implicit operator Fallible<T>(Fallible<Fallible<T>> outer) => outer.Error ? outer.Error : outer.Value;

    public void Deconstruct(out T value, out Error error)
    {
        value = Value;
        error = Error;
    }

    public bool Equals(object? other, IEqualityComparer comparer)
    {
        return other is Fallible<T> errorTuple && Equals(errorTuple);
    }

    public int GetHashCode(IEqualityComparer comparer)
    {
        return HashCode.Combine(
            comparer.GetHashCode(Value!), 
            comparer.GetHashCode(Error));
    }

    public object? this[int index] => 
        index switch
        {
            0 => Value,
            1 => Error,
            _ => throw new IndexOutOfRangeException()
        };

    public int Length => 2;
}
