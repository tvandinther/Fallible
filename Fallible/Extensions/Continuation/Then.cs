namespace FallibleTypes.Extensions.Continuation;

public static partial class FallibleExtensions
{
    /// <summary>
    /// Allows for chaining of non-fallible operations.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="then">An expression to be chained if there is no error.</param>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>If any of the operations fail, the chain stays in an error state.</remarks>
    public static Fallible<TResult> Then<TValue, TResult>(this Fallible<TValue> fallible, Func<TResult> then)
    {
        return fallible.Error ? fallible.Error : then();
    }

    /// <summary>
    /// Chains fallible operations, short-circuiting the evaluation of the second operation if the first operation fails.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="then">A fallible operation.</param>
    /// <typeparam name="TValue">The type of the fallible being chained.</typeparam>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>The fallible returned from the operation.</returns>
    public static Fallible<TResult> Then<TValue, TResult>(this Fallible<TValue> fallible, Func<Fallible<TResult>> then)
    {
        return fallible.Error ? fallible.Error : then();
    }

    /// <inheritdoc cref="Then{TResult}"/>
    /// <typeparam name="TValue">The type of the fallible being chained.</typeparam>
    public static Fallible<TResult> Then<TValue, TResult>(this Fallible<TValue> fallible, Func<TValue, TResult> then)
    {
        return Then(fallible, () => then(fallible.Value));
    }

    /// <summary>
    /// Allows for chaining of fallible operations.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="then">An expression to be chained if there is no error.</param>
    /// <typeparam name="TValue">The type of the fallible being chained.</typeparam>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>If any of the operations fail, the chain stays in an error state.</remarks>
    public static Fallible<TResult> Then<TValue, TResult>(this Fallible<TValue> fallible, Func<TValue, Fallible<TResult>> then)
    {
        return Then(fallible, () => then(fallible.Value));
    }
}