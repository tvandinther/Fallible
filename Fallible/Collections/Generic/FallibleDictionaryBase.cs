using System.Collections;

namespace FallibleTypes.Collections.Generic;

public abstract class FallibleDictionaryBase<TKey, TValue> : FallibleCollection<KeyValuePair<TKey, TValue>>, IFallibleDictionary<TKey, TValue> where TKey : notnull
{
    private readonly IDictionary<TKey, TValue> _dictionary;

    protected internal FallibleDictionaryBase(IDictionary<TKey, TValue> implementation) : base(implementation)
    {
        _dictionary = implementation;
    }

    public Fallible<TValue> this[TKey key]
    {
        get
        {
            try
            {
                return _dictionary[key];
            }
            catch (KeyNotFoundException)
            {
                return new Error("The given key was not present in the dictionary.");
            }
        }
    }

    public ICollection<TKey> Keys => _dictionary.Keys;

    public ICollection<TValue> Values => _dictionary.Values;

    public Fallible<bool> ContainsKey(TKey key)
    {
        try
        {
            return _dictionary.ContainsKey(key);
        }
        catch (ArgumentNullException)
        {
            return new Error("The key is null.");
        }
    }

    public Fallible<Void> Add(TKey key, TValue value)
    {
        try
        {
            _dictionary.Add(key, value);
        }
        catch (ArgumentNullException)
        {
            return new Error("The key is null.");
        }
        catch (ArgumentException)
        {
            return new Error("An element with the same key already exists in the dictionary.");
        }
        catch (NotSupportedException)
        {
            return new Error("The dictionary is read-only.");
        }
        
        return Fallible.Return;
    }

    public Fallible<bool> Remove(TKey key)
    {
        try
        {
            return _dictionary.Remove(key);
        }
        catch (ArgumentNullException)
        {
            return new Error("The key is null.");
        }
        catch (NotSupportedException)
        {
            return new Error("The dictionary is read-only.");
        }
    }
}
