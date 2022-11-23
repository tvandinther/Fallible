using FsCheck;

namespace FallibleTypes.Tests;

internal class Generators
{
    public static Arbitrary<Error> Errors()
    {
        return Arb.From(Arb.Generate<NonNull<string>>().Select(x => new Error(x.Get)));
    }

    public static Arbitrary<Fallible<T>> Fallibles<T>()
    {
        var successFallibles = Arb.From(Arb.Generate<T>().Select(x => (Fallible<T>) x));
        var failedFallibles = Arb.From(Errors().Generator.Select(x => (Fallible<T>) x));
        
        return Arb.From(Gen.OneOf(successFallibles.Generator, failedFallibles.Generator));
    }
    
    public static Arbitrary<Successful<Fallible<T>>> SuccessFallibles<T>()
    {
        return Arb.From(Fallibles<T>().Generator.Select(x => new Successful<Fallible<T>>(x)));
    }
    
    public static Arbitrary<Failed<Fallible<T>>> FailedFallibles<T>()
    {
        return Arb.From(Fallibles<T>().Generator.Select(x => new Failed<Fallible<T>>(x)));
    }
}

internal record class Successful<T>(Fallible<T> Get);
internal record class Failed<T>(Fallible<T> Get);