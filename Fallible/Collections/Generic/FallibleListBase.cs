using System.Collections;

namespace FallibleTypes.Collections.Generic;

public abstract class FallibleListBase<T> : FallibleCollection<T>, IFallibleList<T>
{
    private readonly IList<T> _list;
    
    protected internal FallibleListBase(IList<T> implementation) : base(implementation)
    {
        _list = implementation;
    }

    public Fallible<T> this[int index]
    {
        get
        {
            try
            {
                return _list[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                return new Error("Index out of range");
            }
        }
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public Fallible<Void> Insert(int index, T item)
    {
        try
        {
            _list.Insert(index, item);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new Error("Index is not a valid index in the list");
        }
        catch (NotSupportedException)
        {
            return new Error("Collection is read-only");
        }
        
        return Fallible.Return;
    }

    public Fallible<Void> RemoveAt(int index)
    {
        try
        {
            _list.RemoveAt(index);
        }
        catch (ArgumentOutOfRangeException)
        {
            return new Error("Index is not a valid index in the list");
        }
        catch (NotSupportedException)
        {
            return new Error("Collection is read-only");
        }
        
        return Fallible.Return;
    }
}