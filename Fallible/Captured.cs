namespace FallibleTypes;

public class Captured<TValue>
{
    private readonly TValue _value;
    
    public Captured(TValue value)
    {
        _value = value;
    }

    public LinkedValue<TValue, TResult> If<TResult>(Func<TValue, Fallible<TResult>> func)
    {
        var fallible = func(_value);

        return new LinkedValue<TValue, TResult>(_value, fallible);
    }
}