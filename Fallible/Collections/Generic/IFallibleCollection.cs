using System.Collections;

namespace FallibleTypes.Collections.Generic;

public interface IFallibleCollection<T> : IEnumerable<T>
{
    int Count { get; }
    bool IsReadOnly { get; }
    Fallible<Void> Add(T item);
    Fallible<Void> Clear();
    bool Contains(T item);

    Fallible<Void> CopyTo(T[] array, int arrayIndex);

    Fallible<Void> Remove(T item);
}
