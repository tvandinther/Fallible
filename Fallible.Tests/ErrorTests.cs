using Xunit;

namespace Fallible.Tests;

public class ErrorTests
{
    #region Instantiation Tests

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
    
    #endregion

    #region Conversion Tests

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

    #endregion

    #region Equability Tests

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

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ContainsMessage()
    {
        var expectedSubstring = "Test";
        var error = new Error(expectedSubstring);
        
        var result = error.ToString();
        
        Assert.Contains(expectedSubstring, result);
    }
    
    [Fact]
    public void ToString_ContainsStackTrace()
    {
        var expectedSubstring = "at Fallible.Error..ctor";
        var error = new Error("Test");
        
        var result = error.ToString();
        
        Assert.Contains(expectedSubstring, result);
    }

    #endregion

    #region Message Tests

    [Fact]
    public void Format_CorrectlyFormatsMessage()
    {
        const string expected = "test :test: test";
        var error = new Error("test");
        
        error.Format("test :{0}: test", error.Message);
        
        Assert.Equal(expected, error.Message);
    }
    
    [Fact]
    public void AdditionOperator_CorrectlyPrependsString_WhenStringOnLHS()
    {
        const string expected = "test: appended";
        var error = new Error("test");
        
        error += ": appended";
        
        Assert.Equal(expected, error.Message);
    }

    [Fact]
    public void AdditionOperator_CorrectlyAppendsString_WhenStringOnRHS()
    {
        const string expected = "prepended: test";
        var error = new Error("test");
        
        error = "prepended: " + error;
        
        Assert.Equal(expected, error.Message);
    }

    #endregion
}