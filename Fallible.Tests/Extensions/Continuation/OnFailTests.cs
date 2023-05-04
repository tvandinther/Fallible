using Valuable.Fallible.Extensions.Continuation;
using Xunit;

namespace Valuable.Fallible.Tests.Extensions.Continuation;

public class OnFailTests
{
    [Fact]
    public void Test()
    {
        Fallible<int> fallible = new Error("Test");
        
        
        
        var y = fallible.OnFail(x => x);
    }
}