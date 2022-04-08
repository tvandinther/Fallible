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

    protected LinkedValue(LinkedValue<TValue, TResult> linkedValue)
    {
        Value = linkedValue.Value;
        FallibleObject = linkedValue.FallibleObject;
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
    
    public FinalLinkedValue<TValue, TResult, TFinal> Then<TFinal>(Func<TValue, Fallible<TFinal>> func)
    {
        if (!InErrorState())
            return new FinalLinkedValue<TValue, TResult, TFinal>(this, func(Value));

        return new FinalLinkedValue<TValue, TResult, TFinal>(this, FallibleObject.Error);
    }
    
    public Fallible<TFinal> ThenFinalise<TFinal>(Func<TResult, Fallible<TFinal>> func)
    {
        return !FallibleObject.Error ? func(FallibleObject.Value) : FallibleObject.Error;
    }
    
    public Fallible<TFinal> ThenFinalise<TFinal>(Func<TValue, TFinal> func)
    {
        return !FallibleObject.Error ? func(Value) : FallibleObject.Error;
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