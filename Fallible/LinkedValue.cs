namespace FallibleTypes;

public class LinkedValue<TValue>
{
    internal TValue Value { get; }
    internal Fallible<Void> FallibleObject { get; }

    public LinkedValue(TValue value, Fallible<Void> fallibleObject)
    {
        Value = value;
        FallibleObject = fallibleObject;
    }

    protected LinkedValue(LinkedValue<TValue> linkedValue)
    {
        Value = linkedValue.Value;
        FallibleObject = linkedValue.FallibleObject;
    }

    public LinkedValue<TValue> Or(Func<TValue, Fallible<Void>> func)
    {
        return Link(InErrorState(), func);
    }

    public LinkedValue<TValue> And(Func<TValue, Fallible<Void>> func)
    {
        return Link(!InErrorState(), func);
    }

    public Fallible<TFinal> ContinueWith<TFinal>(Func<TValue, Fallible<TFinal>> func)
    {
        return !FallibleObject.Error ? func(Value) : FallibleObject.Error;
    }
    
    public FinalLinkedValue<TValue, TFinal> Then<TFinal>(Func<TValue, TFinal> func)
    {
        if (!InErrorState())
        {
            // func(Value);
            return new FinalLinkedValue<TValue, TFinal>(this, func(Value));
        }

        return new FinalLinkedValue<TValue, TFinal>(this, FallibleObject.Error);
    }
    
    // public Fallible<TFinal> ThenFinalise<TFinal>(Func<TResult, Fallible<TFinal>> func)
    // {
    //     return !FallibleObject.Error ? func(FallibleObject.Value) : FallibleObject.Error;
    // }
    //
    // public Fallible<TFinal> ThenFinalise<TFinal>(Func<TValue, TFinal> func)
    // {
    //     return !FallibleObject.Error ? func(Value) : FallibleObject.Error;
    // }

    private bool InErrorState()
    {
        return FallibleObject.Error;
    }

    private LinkedValue<TValue> Link(bool shouldExecute, Func<TValue, Fallible<Void>> func)
    {
        return shouldExecute ? new LinkedValue<TValue>(Value, func(Value)) : this;
    }
}