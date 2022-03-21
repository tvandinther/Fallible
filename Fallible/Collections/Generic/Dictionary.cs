namespace FallibleTypes.Collections.Generic;

public class Dictionary<TKey, TValue> : FallibleDictionaryBase<TKey, TValue> where TKey : notnull
{
    public new System.Collections.Generic.Dictionary<TKey, TValue> Implementation { get; }

    public Dictionary() : base(new System.Collections.Generic.Dictionary<TKey, TValue>())
    {
        Implementation = Implementation!;
    }
}