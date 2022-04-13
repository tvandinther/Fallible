using FallibleTypes.Extensions.Continuation;
using Xunit;

namespace FallibleTypes.Tests.Extensions;

public class RecoverTests
{
    [Fact]
    public void Test()
    {
        Fallible<int> fallible = new Error("Test");
        
        var y = fallible.OnFail(x => x);
    }
}