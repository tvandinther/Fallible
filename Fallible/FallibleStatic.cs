using System.Runtime.CompilerServices;

namespace FallibleTypes;

/// <summary>
/// Utility class for working with Fallible types.
/// </summary>
public static class Fallible
{
    /// <summary>
    /// Returns a fallible <see cref="Void"/> instance.
    /// </summary>
    /// <remarks>This is the only way to instantiate <see cref="Void"/>.</remarks>
    public static Fallible<Void> Return => new Void();

    /// <summary>
    /// Will execute an operation and try to catch any exceptions and returning an error if caught.
    /// </summary>
    /// <param name="try">The operation to execute.</param>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    public static Fallible<TResult> Try<TResult>(Func<TResult> @try, [CallerArgumentExpression("try")] string expression = "")
    {
        try
        {
            var value = @try();
            return value;
        }
        catch (Exception e)
        {
            return new Error($"{expression} threw {e.GetType().Name}: {e.Message}");
        }
    }

    /// <summary>
    /// Will execute a fallible operation and return the result. If the operation fails, the errorMessage will be
    /// prepended.
    /// </summary>
    /// <param name="try">The expression to execute.</param>
    /// <param name="errorMessage">An optional message to prepend to the error on failure.</param>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>Allows for simple conditional wrapping of error messages.</remarks>
    public static Fallible<TResult> Try<TResult>(Func<Fallible<TResult>> @try, string errorMessage = "")
    {
        var (value, error) = @try();
        if (error) return errorMessage + error;

        return value;
    }
    
    /// <summary>
    /// Allows for chaining of non-fallible operations.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="then">The expression to be chained if there is no error.</param>
    /// <param name="errorMessage">An optional message to prepend to the error on failure.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>If any of the operations fail, the chain stays in an error state.</remarks>
    public static Fallible<TResult> Then<TIn, TResult>(this Fallible<TIn> fallible, Func<TIn, TResult> then, string errorMessage = "")
    {
        var (value, error) = fallible;
        if (error) return errorMessage + error;
    
        return then(value);
    }
    
    /// <summary>
    /// Allows for chaining of fallible operations.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="then">The expression to be chained if there is no error.</param>
    /// <param name="errorMessage">An optional message to prepend to the error on failure.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>If any of the operations fail, the chain stays in an error state.</remarks>
    public static Fallible<TResult> Then<TIn, TResult>(this Fallible<TIn> fallible, Func<TIn, Fallible<TResult>> then, string errorMessage = "")
    {
        var (value, error) = fallible;
        if (error) return errorMessage + error;

        return then(value);
    }
    
    /// <summary>
    /// Allows for chaining of expressions that are executed if the chain is in a failure state. Chains of OnFail will
    /// continue to execute until exhausted.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="onFail">The expression to be chained if there is an error.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <returns>The original fallible result.</returns>
    /// <remarks>This will propagate a modified error object. Useful for modifying error messages.</remarks>
    public static Fallible<TIn> OnFail<TIn>(this Fallible<TIn> fallible, Func<Error, Error> onFail)
    {
        var (_, error) = fallible;
        return error ? onFail(error) : fallible;
    }
    
    /// <summary>
    /// Allows for chaining of expressions that are executed if the chain is in a failure state. Chains of OnFail will
    /// continue to execute until exhausted.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="onFail">The expression to be chained if there is an error.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <returns>The original fallible result.</returns>
    public static Fallible<TIn> OnFail<TIn>(this Fallible<TIn> fallible, Action<Error> onFail)
    {
        var (_, error) = fallible;
        if (error) onFail(error);

        return fallible;
    }
}