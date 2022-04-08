using Xunit;

namespace FallibleTypes.Tests.Extensions.Logical;

public class Tests
{
    [Fact]
    public void Test()
    {
        var greaterThanZero = Fallible<bool>(int x) => x > 0;
        var equalTo22 = Fallible<bool>(int x) => x == 22;
        var add1 = Fallible<int>(int x) => x + 1;
        var add2 = Fallible<int>(int x) => x + 2;
        var boolToString = Fallible<string>(bool x) => x.ToString();
        var intToString = Fallible<string>(int x) => x.ToString();
        
        var x = Fallible.About(42)
            .If(greaterThanZero).Then(add1)
            .Or(equalTo22).Then(add2)
            .ThenFinalise(intToString);
        
        Assert.Equal("True", x.Value);
    }
}