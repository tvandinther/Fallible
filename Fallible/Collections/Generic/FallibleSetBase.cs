namespace FallibleTypes.Collections.Generic;

public abstract class FallibleSetBase<T> : FallibleCollection<T>, IFallibleSet<T>
{
    private readonly ISet<T> _set;

    protected FallibleSetBase(ISet<T> implementation) : base(implementation)
    {
        _set = implementation;
    }

    public new bool Add(T item)
    {
        return _set.Add(item);
    }

    public Fallible<Void> UnionWith(IEnumerable<T> other)
    {
        return Try(() => _set.UnionWith(other));
    }

    public Fallible<Void> IntersectWith(IEnumerable<T> other)
    {
        return Try(() => _set.IntersectWith(other));
    }

    public Fallible<Void> ExceptWith(IEnumerable<T> other)
    {
        return Try(() => _set.ExceptWith(other));
    }

    public Fallible<Void> SymmetricExceptWith(IEnumerable<T> other)
    {
        return Try(() => _set.SymmetricExceptWith(other));
    }

    public Fallible<bool> IsSubsetOf(IEnumerable<T> other)
    {
        return Try(() => _set.IsSubsetOf(other));
    }

    public Fallible<bool> IsSupersetOf(IEnumerable<T> other)
    {
        return Try(() => _set.IsSupersetOf(other));
    }

    public Fallible<bool> IsProperSupersetOf(IEnumerable<T> other)
    {
        return Try(() => _set.IsProperSupersetOf(other));
    }

    public Fallible<bool> IsProperSubsetOf(IEnumerable<T> other)
    {
        return Try(() => _set.IsProperSubsetOf(other));
    }

    public Fallible<bool> Overlaps(IEnumerable<T> other)
    {
        return Try(() => _set.Overlaps(other));
    }

    public Fallible<bool> SetEquals(IEnumerable<T> other)
    {
        return Try(() => _set.SetEquals(other));
    }

    private static Fallible<Void> Try(Action func)
    {
        try
        {
            func();
        }
        catch (ArgumentNullException)
        {
            return new Error("other is null");
        }
        
        return Fallible.Return;
    }
    
    private static Fallible<bool> Try(Func<bool> func)
    {
        try
        {
            return func();
        }
        catch (ArgumentNullException)
        {
            return new Error("other is null");
        }
    }
}