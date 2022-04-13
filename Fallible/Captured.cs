namespace FallibleTypes;

public class Captured<TValue>
{
    private readonly TValue _value;
    
    public Captured(TValue value)
    {
        _value = value;
    }

    public LinkedValue<TValue> If(Func<TValue, Fallible<Void>> func)
    {
        var fallible = func(_value);

        return new LinkedValue<TValue>(_value, fallible);
    }
}