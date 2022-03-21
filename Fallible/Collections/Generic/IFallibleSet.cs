namespace FallibleTypes.Collections.Generic;

public interface IFallibleSet<T> : IFallibleCollection<T>
{
    new bool Add(T item);
    Fallible<Void> UnionWith(IEnumerable<T> other);
    Fallible<Void> IntersectWith(IEnumerable<T> other);
    Fallible<Void> ExceptWith(IEnumerable<T> other);
    Fallible<Void> SymmetricExceptWith(IEnumerable<T> other);
    Fallible<bool> IsSubsetOf(IEnumerable<T> other);
    Fallible<bool> IsSupersetOf(IEnumerable<T> other);
    Fallible<bool> IsProperSupersetOf(IEnumerable<T> other);
    Fallible<bool> IsProperSubsetOf(IEnumerable<T> other);
    Fallible<bool> Overlaps(IEnumerable<T> other);
    Fallible<bool> SetEquals(IEnumerable<T> other);
}
