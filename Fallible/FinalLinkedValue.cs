namespace FallibleTypes;

public class FinalLinkedValue<TValue, TFinal> : LinkedValue<TValue>
{
    private LinkedValue<TValue> _linkedValue;
    private Fallible<TFinal> _final;
    
    internal FinalLinkedValue(LinkedValue<TValue> linkedValue, Fallible<TFinal> final) : base(linkedValue)
    {
        _linkedValue = linkedValue;
        _final = final;
    }
    
    public Fallible<TNew> ThenFinally<TNew>(Func<TFinal, Fallible<TNew>> func)
    {
        if (!_final.Error) return func(_final.Value);

        // return _linkedValue.FallibleObject.Error;
        return _final.Error;
    }
}