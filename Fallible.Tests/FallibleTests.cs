using Fallible;
using Xunit;

namespace Errors.Tests;

public class FallibleTests
{
    [Fact]
    public void WhenCreated_ContainsError()
    {
        var error = new Error("Wrong Number");

        Fallible<int> fallible = error;
        
        Assert.Equal(error, fallible.Error);
    }
    
    [Fact]
    public void WhenCreated_ContainsDefaultValue_WhenValueType()
    {
        var error = new Error("Wrong Number");

        Fallible<int> fallible = error;
        
        Assert.Equal(default, fallible.Value);
    }
    
    [Fact]
    public void WhenCreated_ContainsDefaultValue_WhenReferenceType()
    {
        var error = new Error("Wrong Number");

        Fallible<object> fallible = error;

        Assert.Equal(default, fallible.Value);
        Assert.Null(fallible.Value);
    }

    [Fact]
    public void CanBeDeconstructed_ValueIsDefault()
    {
        var expectedError = new Error("Wrong Number");
        Fallible<int> fallible = expectedError;
        
        var (number, _) = fallible;
        
        Assert.Equal(default, number);
    }
    
    [Fact]
    public void CanBeDeconstructed_ErrorIsExpected()
    {
        var expectedError = new Error("Wrong Number");
        Fallible<int> fallible = expectedError;
        
        var (_, error) = fallible;
        
        Assert.Equal(expectedError, error);
    }
    
    [Fact]
    public void CanBeDeconstructed_ValueIsExpected()
    {
        var expectedValue = 42;
        
        var (value, _) = ReturnValue_ViaImplicitConversion(expectedValue);
        
        Assert.Equal(expectedValue, value);
    }
    
    [Fact]
    public void CanBeImplicitlyConverted_FromValue()
    {
        Fallible<int> fallible = 42;
        
        Assert.IsType<Fallible<int>>(fallible);
    }
    
    [Fact]
    public void CanBeImplicitlyConverted_FromError()
    {
        Fallible<int> fallible = new Error("Test");
        
        Assert.IsType<Fallible<int>>(fallible);
    }
    
    [Fact]
    public void CanBeImplicitlyConverted_FromTuple_WhenErrorIsNull()
    {
        Fallible<int> fallible = (42, null);
        
        Assert.IsType<Fallible<int>>(fallible);
    }
    
    [Fact]
    public void CanBeImplicitlyConverted_FromTuple_WhenValueIsDefault()
    {
        var error = new Error("Wrong Number");
        
        Fallible<int> fallible = (default, error);
        
        Assert.IsType<Fallible<int>>(fallible);
    }
    
    [Fact]
    public void CanBeImplicitlyConverted_FromTuple_WhenValueIsNull()
    {
        var error = new Error("Wrong Number");
        
        Fallible<object> fallible = (null, error);
        
        Assert.IsType<Fallible<object>>(fallible);
    }

    private static Fallible<T> ReturnValue_ViaImplicitConversion<T>(T value)
    {
        return value;
    }
}