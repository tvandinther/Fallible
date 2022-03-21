namespace FallibleTypes.Collections.Generic;

public class HashSet<T> : FallibleSetBase<T>
{
    public HashSet() : base(new System.Collections.Generic.HashSet<T>())
    {
    }
}