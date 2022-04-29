using FallibleTypes.Extensions.Continuation;
using Xunit;

namespace FallibleTypes.Tests.Extensions.Continuation;

public class OnFailTests
{
    [Fact]
    public void Test()
    {
        Fallible<int> fallible = new Error("Test");
        
        var y = fallible.OnFail(x => x);
    }
}