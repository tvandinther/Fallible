namespace FallibleTypes;

public class LinkedBool<TValue>
{
    private TValue Value { get; }
    public Fallible<Void> FallibleObject { get; }
    
    public LinkedBool(TValue value, Fallible<Void> fallibleObject)
    {
        Value = value;
        FallibleObject = fallibleObject;
    }

    public LinkedBool<TValue> Or(Func<TValue, bool> func)
    {
        return Link(InErrorState(), func);
    }
    
    public LinkedBool<TValue> And(Func<TValue, bool> func)
    {
        return Link(!InErrorState(), func);
    }
    
    public Fallible<TFinal> Then<TFinal>(Func<TFinal> func)
    {
        return !FallibleObject.Error ? func() : FallibleObject.Error;
    }
    
    private bool InErrorState()
    {
        return FallibleObject.Error;
    }
    
    private LinkedBool<TValue> Link(bool shouldExecute, Func<TValue, bool> func)
    {
        return shouldExecute ? new LinkedBool<TValue>(Value, Fallible.If(func(Value))) : this;
    }
}