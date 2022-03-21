namespace FallibleTypes.Collections.Generic;

public interface IFallibleList<T> : IFallibleCollection<T>
{
    Fallible<T> this[int index] { get; }
    int IndexOf(T item);
    Fallible<Void> Insert(int index, T item);
    Fallible<Void> RemoveAt(int index);
}
