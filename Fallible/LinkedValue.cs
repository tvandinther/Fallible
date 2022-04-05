namespace FallibleTypes;

public class LinkedValue<TValue, TResult>
{
    private TValue Value { get; }
    private Fallible<TResult> FallibleObject { get; }
    
    public LinkedValue(TValue value, Fallible<TResult> fallibleObject)
    {
        Value = value;
        FallibleObject = fallibleObject;
    }

    public LinkedValue<TValue, TResult> Or(Func<TValue, Fallible<TResult>> func)
    {
        return Link(InErrorState(), func);
    }

    public LinkedValue<TValue, TResult> And(Func<TValue, Fallible<TResult>> func)
    {
        return Link(!InErrorState(), func);
    }

    public Fallible<TFinal> ContinueWith<TFinal>(Func<TValue, Fallible<TFinal>> func)
    {
        return !FallibleObject.Error ? func(Value) : FallibleObject.Error;
    }
    
    public Fallible<TFinal> Then<TFinal>(Func<TResult, Fallible<TFinal>> func)
    {
        return !FallibleObject.Error ? func(FallibleObject.Value) : FallibleObject.Error;
    }

    private bool InErrorState()
    {
        return FallibleObject.Error;
    }

    private LinkedValue<TValue, TResult> Link(bool shouldExecute, Func<TValue, Fallible<TResult>> func)
    {
        return shouldExecute ? new LinkedValue<TValue, TResult>(Value, func(Value)) : this;
    }
}