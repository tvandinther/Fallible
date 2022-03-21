using System.Collections;

namespace FallibleTypes.Collections.Generic;

public interface IFallibleDictionary<TKey, TValue> : IFallibleCollection<KeyValuePair<TKey, TValue>> where TKey : notnull
{ 
    Fallible<TValue> this[TKey key] { get; }
    ICollection<TKey> Keys { get; }
    ICollection<TValue> Values { get; }
    Fallible<bool> ContainsKey(TKey key);
    Fallible<Void> Add(TKey key, TValue value);
    Fallible<bool> Remove(TKey key);

}
