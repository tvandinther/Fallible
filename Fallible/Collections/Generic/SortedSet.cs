namespace FallibleTypes.Collections.Generic;

public class SortedSet<T> : FallibleSetBase<T>
{
    public SortedSet() : base(new System.Collections.Generic.SortedSet<T>())
    {
    }
}