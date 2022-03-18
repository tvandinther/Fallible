using System.Runtime.CompilerServices;

namespace FallibleTypes;

public static class Fallible
{
    public static Fallible<TResult> Try<TResult>(Func<TResult> action, [CallerArgumentExpression("action")] string expression = "")
    {
        try
        {
            var value = action();
            return value;
        }
        catch (Exception e)
        {
            return new Error($"{expression} threw {e.GetType().Name}: {e.Message}");
        }
    }

    public static Fallible<Void> Return => new Void();
}