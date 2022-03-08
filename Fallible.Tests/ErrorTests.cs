using Fallible;
using Xunit;

namespace Errors.Tests;

public class ErrorTests
{
    // Instantiation
    
    [Fact]
    public void WhenCreated_ShouldHaveMessage()
    {
        const string expected = "Test";
        var error = new Error(expected);

        Assert.Equal(expected, error.Message);
    }
    
    [Fact]
    public void WhenCreated_ShouldHaveStackTrace()
    {
        var expectedStackTraceSubstring = "at Fallible.Error..ctor";
        
        var error = new Error("Test");

        Assert.Contains(expectedStackTraceSubstring, error.StackTrace);
    }
    
    // Conversion

    [Fact]
    public void WhenCreated_BoolConversion_ShouldReturnTrue()
    {
        var error = new Error("Test");
        
        Assert.True(error);
    }
    
    [Fact]
    public void WhenNull_BoolConversion_ShouldReturnFalse()
    {
        Error error = default!;
        
        Assert.False(error);
    }

    // Equability
    
    [Fact]
    public void WhenEquated_GivenSameInstance_ShouldReturnTrue()
    {
        var error = new Error("Test");
        var sameError = error;

        var result = error.Equals(sameError);
        
        Assert.True(result);
    }
    
    [Fact]
    public void WhenEquated_GivenDifferentInstances_ShouldReturnFalse()
    {
        var error = new Error("Test");
        var differentError = new Error("Different");
        
        var result = error.Equals(differentError);
        
        Assert.False(result);
    }
    
    [Fact]
    public void WhenEquated_GivenNull_ShouldReturnFalse()
    {
        var error = new Error("Test");
        
        var result = error.Equals(null!);
        
        Assert.False(result);
    }
    
    [Fact]
    public void WhenEquated_GivenDifferentType_ShouldReturnFalse()
    {
        var error = new Error("Test");
        
        var result = error.Equals(new object());
        
        Assert.False(result);
    }

    [Fact]
    public void WhenEquated_GivenSameValues_ShouldReturnFalse()
    {
        var error = new Error("Test");
        var sameError = new Error("Test");
        
        var result = error.Equals(sameError);
        
        Assert.False(result);
    }
    
    [Fact]
    public void WhenEquated_GivenSameLocationOfInstantiation_ShouldReturnTrue()
    {
        var createError = () => new Error("Test");
        var error1 = createError();
        var error2 = createError();
        
        var result = error1.Equals(error2);
        
        Assert.True(result);
    }
}