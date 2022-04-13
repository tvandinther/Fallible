using FallibleTypes.Extensions;
using Xunit;

namespace FallibleTypes.Tests.Extensions.Logical;

public class Tests
{
    [Fact]
    public void Test()
    {
        var greaterThanZero = Fallible<Void>(int x) 
            => x > 0 ? Fallible.Return : new Error("x must be greater than zero");
        var equalTo22 = Fallible<Void>(int x) 
            => x == 22 ? Fallible.Return : new Error("x must be equal to 22");
        var add1 = Fallible<int>(int x) => x + 1;
        var add2 = Fallible<int>(int x) => x + 2;
        var intToString = string(int x) => x.ToString();
        
        var x = Fallible.About(42)
            .If(greaterThanZero).Then(add1)
            .Or(equalTo22).Then(add2)
            .ThenFinally(intToString.AsFallible());

        // Failing because after the first then, or is skipped (as it should due to being in a true state) but the 2nd
        // "then" is triggered due to the true state. It should instead be scoped to the logical chain preceeding it.

        Assert.Equal("43", x.Value);
    }
}