namespace FallibleTypes;

public class FinalLinkedValue<TValue, TResult, TFinal> : LinkedValue<TValue, TResult>
{
    private LinkedValue<TValue, TResult> _linkedValue;
    private Fallible<TFinal> _final;
    
    internal FinalLinkedValue(LinkedValue<TValue, TResult> linkedValue, Fallible<TFinal> final) : base(linkedValue)
    {
        _linkedValue = linkedValue;
        _final = final;
    }
}