namespace FallibleTypes.Collections.Generic;

public class List<T> : FallibleListBase<T>
{
    internal List() : base(new System.Collections.Generic.List<T>())
    {
    }
}