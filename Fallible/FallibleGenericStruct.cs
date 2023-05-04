using System.Collections;

namespace Valuable.Fallible;

/// <summary>
/// A record struct that represents a return type with a possible failure state.
/// </summary>
/// <typeparam name="T">The return type.</typeparam>
/// <remarks><see cref="Fallible{T}"/> will only ever be in a succeeded state or a failed state.</remarks>
public readonly record struct Fallible<T> : IStructuralEquatable
{
    /// <summary>
    /// The value.
    /// </summary>
    /// <remarks>Will have a default value if in a failed state.</remarks>
    public T Value { get; }
    
    /// <summary>
    /// A reference to the Error.
    /// </summary>
    /// <remarks>Will be null if in a succeeded state.</remarks>
    public Error Error { get; }

    private Fallible(T value, Error error)
    {
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Creates <see cref="Fallible{T}"/> in a failed state."/>
    /// </summary>
    /// <param name="error">The error to be contained.</param>
    /// <returns>A fallible object.</returns>
    public static implicit operator Fallible<T>(Error error) => new(default!, error);
    
    /// <summary>
    /// Creates <see cref="Fallible{T}"/> in a succeeded state.
    /// </summary>
    /// <param name="value">The value to be contained.</param>
    /// <returns>A fallible object.</returns>
    public static implicit operator Fallible<T>(T value) => new(value, default!);
    
    /// <summary>
    /// Unwraps the value if in a succeeded state or just the error if in a failed state.
    /// </summary>
    /// <param name="outer">The outer <see cref="Fallible{T}"/>.</param>
    /// <returns>An unwrapped <see cref="Fallible{T}"/> object.</returns>
    public static implicit operator Fallible<T>(Fallible<Fallible<T>> outer) => outer.Error ? outer.Error : outer.Value;

    /// <summary>
    /// Deconstructs <see cref="Fallible{T}"/> into a value and error.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="error">The error.</param>
    /// <remarks>Only one of value or error will be populated. Perform a boolean check on error before using the value.</remarks>
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
}
