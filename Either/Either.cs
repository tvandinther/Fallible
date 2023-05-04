using System.Collections;

namespace Either;

/// <summary>
/// Represents a value of one of two possible types.
/// </summary>
/// <typeparam name="T1">The Left type. Typically an error type.</typeparam>
/// <typeparam name="T2">The Right type. Typically a non-error type.</typeparam>
/// <remarks>A handy mnemonic: Right is right!</remarks>
public readonly record struct Either<T1, T2> : IStructuralEquatable
{
    private readonly T1 _left;
    private readonly T2 _right;
    public bool IsLeft { get; }
    public bool IsRight => !IsLeft;

    private Either(T1 left, T2 right, bool isLeft)
    {
        _left = left;
        _right = right;
        IsLeft = isLeft;
    }
    
    public static implicit operator Either<T1, T2>(T1 left) => new(left, default!, true);
    public static implicit operator Either<T1, T2>(T2 right) => new(default!, right, false);
    
    public T Match<T>(Func<T1, T> left, Func<T2, T> right) => IsLeft ? left(_left) : right(_right);
    
    public T1 LeftOrDefault(T1 defaultValue) => IsLeft ? _left : defaultValue;
    public T2 RightOrDefault(T2 defaultValue) => IsRight ? _right : defaultValue;
    
    public bool Equals(object? other, IEqualityComparer comparer)
    {
        throw new NotImplementedException();
    }

    public int GetHashCode(IEqualityComparer comparer)
    {
        throw new NotImplementedException();
    }
}