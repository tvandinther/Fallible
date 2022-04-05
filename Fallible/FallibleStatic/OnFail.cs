namespace FallibleTypes;

public static partial class Fallible
{
    /// <summary>
    /// Allows for chaining of expressions that are executed if the chain is in a failure state. Chains of OnFail will
    /// continue to execute until exhausted.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="onFail">An expression to be chained if there is an error.</param>
    /// <typeparam name="TValue">The type of the fallible being chained.</typeparam>
    /// <returns>The original fallible result.</returns>
    public static Fallible<TValue> OnFail<TValue>(this Fallible<TValue> fallible, Func<Error, Fallible<TValue>> onFail)
    {
        return OnFail(fallible, () => onFail(fallible.Error));
    }
    
    /// <summary>
    /// Allows for chaining of expressions that are executed if the chain is in a failure state. Chains of OnFail will
    /// continue to execute until exhausted.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="onFail">An expression to be chained if there is an error.</param>
    /// <typeparam name="TValue">The type of the fallible being chained.</typeparam>
    /// <returns>The original fallible result.</returns>
    /// <remarks>This will propagate a modified error object. Useful for modifying error messages.</remarks>
    public static Fallible<TValue> OnFail<TValue>(this Fallible<TValue> fallible, Func<Fallible<TValue>> onFail)
    {
        if (fallible.Error) onFail();
        
        return fallible;
    }
}