using System.Collections;

namespace FallibleTypes.Collections.Generic;

public abstract class FallibleCollection<T> : IFallibleCollection<T>
{
    protected readonly ICollection<T> Implementation;

    protected internal FallibleCollection(ICollection<T> implementation)
    {
        Implementation = implementation;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return Implementation.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable) Implementation).GetEnumerator();
    }

    public int Count => Implementation.Count;
    public bool IsReadOnly => Implementation.IsReadOnly;
    public Fallible<Void> Add(T item)
    {
        try
        {
            Implementation.Add(item);
        }
        catch (NotSupportedException)
        {
            return new Error("The collection is read-only.");
        }
        
        return Fallible.Return;
    }

    public Fallible<Void> Clear()
    {
        try
        {
            Implementation.Clear();
        }
        catch (NotSupportedException)
        {
            return new Error("The collection is read-only.");
        }
        
        return Fallible.Return;
    }

    public bool Contains(T item)
    {
        try
        {
            return Implementation.Contains(item);
        }
        catch (ArgumentNullException)
        {
            return new Error("The item is null.");
        }
    }

    public Fallible<Void> CopyTo(T[] array, int arrayIndex)
    {
        try
        {
            Implementation.CopyTo(array, arrayIndex);
        }
        catch (ArgumentNullException)
        {
            return new Error("The array is null.");
        }
        catch (ArgumentOutOfRangeException)
        {
            return new Error("arrayIndex is less than zero.");
        }
        catch (ArgumentException)
        {
            return new Error("The number of elements in the source collection is greater than the available space from arrayIndex to the end of the destination array.");
        }
        
        return Fallible.Return;
    }

    public Fallible<Void> Remove(T item)
    {
        try
        {
            Implementation.Remove(item);
            
            return Fallible.Return;
        }
        catch (NotSupportedException)
        {
            return new Error("The collection is read-only.");
        }
    }
}