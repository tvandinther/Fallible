using System.Runtime.CompilerServices;

namespace FallibleTypes;

/// <summary>
/// Utility class for working with Fallible types.
/// </summary>
public static partial class Fallible
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