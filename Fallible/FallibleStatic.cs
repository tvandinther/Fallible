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
    /// <param name="try">An operation to execute.</param>
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
    /// <param name="try">An expression to execute.</param>
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
    /// <param name="then">An expression to be chained if there is no error.</param>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>If any of the operations fail, the chain stays in an error state.</remarks>
    public static Fallible<TResult> Then<TResult>(this Fallible<Void> fallible, Func<TResult> then)
    {
        var (_, error) = fallible;
        if (error) return error;
    
        return then();
    }
    
    /// <inheritdoc cref="Then{TResult}"/>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    public static Fallible<TResult> Then<TIn, TResult>(this Fallible<TIn> fallible, Func<TIn, TResult> then)
    {
        var (value, error) = fallible;
        if (error) return error;
    
        return then(value);
    }

    /// <summary>
    /// Allows for chaining of fallible operations.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="then">An expression to be chained if there is no error.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <typeparam name="TResult">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    /// <remarks>If any of the operations fail, the chain stays in an error state.</remarks>
    public static Fallible<TResult> Then<TIn, TResult>(this Fallible<TIn> fallible, Func<TIn, Fallible<TResult>> then)
    {
        return fallible.Error ? fallible.Error : then(fallible.Value);
    }
    
    /// <summary>
    /// Allows for chaining of expressions that are executed if the chain is in a failure state. Chains of OnFail will
    /// continue to execute until exhausted.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="onFail">An expression to be chained if there is an error.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <returns>The original fallible result.</returns>
    /// <remarks>This will propagate a modified error object. Useful for modifying error messages.</remarks>
    public static Fallible<TIn> OnFail<TIn>(this Fallible<TIn> fallible, Func<Error, Fallible<TIn>> onFail)
    {
        return fallible.Error ? onFail(fallible.Error) : fallible;
    }
    
    /// <summary>
    /// Allows for chaining of expressions that are executed if the chain is in a failure state. Chains of OnFail will
    /// continue to execute until exhausted.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="onFail">An expression to be chained if there is an error.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <returns>The original fallible result.</returns>
    public static Fallible<TIn> OnFail<TIn>(this Fallible<TIn> fallible, Action<Error> onFail)
    {
        if (fallible.Error) onFail(fallible.Error);

        return fallible;
    }

    /// <summary>
    /// Same as <see cref="Try{TResult}(System.Func{TResult},string)"/> without a parameter for prepending the error
    /// message. Used to start a logical chain of fallible operations.
    /// </summary>
    /// <param name="func">A fallible operation.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible result.</returns>
    public static Fallible<T> If<T>(Func<Fallible<T>> func) => func();
    
    /// <summary>
    /// Creates a fallible result from an expression.
    /// </summary>
    /// <param name="expression">An expression to evaluate.</param>
    /// <returns>A fallible in error state if the expression is false.</returns>
    public static Fallible<Void> If(bool expression, [CallerArgumentExpression("expression")] string callerExpression = "")
    {
        return expression ? Return : new Error($"{callerExpression} was false");
    }

    /// <summary>
    /// Will evaluate the expression if the chain is in a failed state.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="expression">An expression to evaluate.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible in error state if the expression is false.</returns>
    public static Fallible<T> OrIf<T>(this Fallible<T> fallible, bool expression, [CallerArgumentExpression("expression")] string callerExpression = "")
    {
        var (value, error) = fallible;

        if (error) return expression ? value : new Error($"{callerExpression} was false");
        
        return fallible;
    }

    /// <summary>
    /// Will evaluate the expression if the chain is in a succeeded state.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="expression">An expression to evaluate.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible in error state if the expression is false.</returns>
    public static Fallible<T> AndIf<T>(this Fallible<T> fallible, bool expression, [CallerArgumentExpression("expression")] string callerExpression = "")
    {
        var (value, error) = fallible;
        if (error) return error;
        
        return expression ? fallible : new Error($"{callerExpression} was false");
    }
    
    /// <summary>
    /// Will evaluate the expression if the chain is in a succeeded state.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="expressionFunc">The expression function to evaluate the value against.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible in error state if the expression is false.</returns>
    /// <remarks>Receives the value from the chain.</remarks>
    public static Fallible<T> AndIf<T>(this Fallible<T> fallible, Func<T, bool> expressionFunc, [CallerArgumentExpression("expressionFunc")] string callerExpression = "")
    {
        var (value, error) = fallible;
        if (error) return error;
        
        return expressionFunc(value) ? fallible : new Error($"{callerExpression} was false");
    }

    /// <summary>
    /// Chains fallible operations, short-circuiting the evaluation of the second operation if the first operation succeeds.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="func">A fallible operation.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>The fallible returned from the operation.</returns>
    public static Fallible<T> Or<T>(this Fallible<T> fallible, Func<Fallible<T>> func)
    {
        return fallible.Error ? func() : fallible;
    }

    /// <summary>
    /// Chains fallible operations, short-circuiting the evaluation of the second operation if the first operation fails.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="func">A fallible operation.</param>
    /// <typeparam name="TIn">The type of the fallible being chained.</typeparam>
    /// <typeparam name="TOut">The type of the fallible being returned.</typeparam>
    /// <returns>The fallible returned from the operation.</returns>
    public static Fallible<TOut> And<TIn, TOut>(this Fallible<TIn> fallible, Func<Fallible<TOut>> func)
    {
        return fallible.Error ? fallible.Error : func();
    }

    #region Covariance and Contravariance

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

    #endregion
}