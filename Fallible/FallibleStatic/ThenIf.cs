using System.Runtime.CompilerServices;

namespace FallibleTypes;

public static partial class Fallible
{
    /// <summary>
    /// Will evaluate the expression if the chain is in a succeeded state.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="expression">An expression to evaluate.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible in error state if the expression is false.</returns>
    public static Fallible<T> ThenIf<T>(this Fallible<T> fallible, bool expression, [CallerArgumentExpression("expression")] string callerExpression = "")
    {
        var (_, error) = fallible;
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
    public static Fallible<T> ThenIf<T>(this Fallible<T> fallible, Func<T, bool> expressionFunc, [CallerArgumentExpression("expressionFunc")] string callerExpression = "")
    {
        var (value, error) = fallible;
        if (error) return error;
        
        return expressionFunc(value) ? fallible : new Error($"{callerExpression} was false");
    }
    
    public static Fallible<T> ThenIf<T>(this Fallible<T> fallible, Func<bool> expressionFunc, [CallerArgumentExpression("expressionFunc")] string callerExpression = "")
    {
        var (_, error) = fallible;
        if (error) return error;
        
        return expressionFunc() ? fallible : new Error($"{callerExpression} was false");
    }
}