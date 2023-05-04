using System.Runtime.CompilerServices;

namespace Valuable.Fallible.Extensions.Continuation;

public static partial class FallibleExtensions
{
    /// <summary>
    /// Will evaluate the expression if the chain is in a failed state.
    /// </summary>
    /// <param name="fallible">The fallible being chained.</param>
    /// <param name="expression">An expression to evaluate.</param>
    /// <typeparam name="T">The type of the fallible being returned.</typeparam>
    /// <returns>A fallible in error state if the expression is false.</returns>
    public static Fallible<T> OnFailIf<T>(this Fallible<T> fallible, bool expression, [CallerArgumentExpression("expression")] string callerExpression = "")
    {
        var (value, error) = fallible;

        if (error) return expression ? value : new Error($"{callerExpression} was false");
        
        return fallible;
    }
    
    public static Fallible<T> OnFailIf<T>(this Fallible<T> fallible, Func<bool> expression, [CallerArgumentExpression("expression")] string callerExpression = "")
    {
        return OnFailIf(fallible, expression(), callerExpression);
    }
}