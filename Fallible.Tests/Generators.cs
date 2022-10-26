using FsCheck;

namespace FallibleTypes.Tests;

public class Generators
{
    public static Arbitrary<Error> Error()
    {
        return Arb.From(Arb.Generate<NonNull<string>>().Select(x => new Error(x.Get)));
    }
}