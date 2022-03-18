using System;
using Xunit;

namespace FallibleTypes.Tests;

public class FallibleTests
{
    #region Instantiation Tests

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

    #endregion

    #region Deconstruction Tests

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
        Fallible<int> fallible = expectedValue;

        var (value, _) = fallible;
        
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void CanBeDeconstructed_ShouldHaveNullResult_WhenVoidReturn()
    {
        Fallible<Void> fallible = new Error("Wrong Number");
        
        var (result, _) = fallible;
        
        Assert.Null(result);
    }

    #endregion

    #region Conversion Tests

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
    public void CanBeImplicitlyConverted_FromVoid_WhenReturning()
    {
        var func = Fallible<Void>() => Fallible.Return;

        Fallible<Void> fallible = func();
        
        Assert.IsType<Fallible<Void>>(fallible);

    }
    
    [Fact]
    public void WhenWrapped_ShouldUnwrapItselfImplicitly()
    {
        Fallible<int> inner = 42;
        Fallible<Fallible<int>> outer = inner;
        
        Fallible<int> result = outer;

        Assert.IsType<Fallible<int>>(result);
    }
    
    [Fact]
    public void WhenImplicitlyUnwrapped_ShouldContainInnerValue()
    {
        const int expectedValue = 42;
        Fallible<int> inner = expectedValue;
        Fallible<Fallible<int>> outer = inner;
        
        Fallible<int> result = outer;

        Assert.Equal(expectedValue, result.Value);
    }

    [Fact]
    public void WhenImplicitlyUnwrapped_ShouldContainCorrectError_WhenInnerErrorIsPresent()
    {
        var expectedError = new Error("Inner Error");
        Fallible<int> inner = expectedError;
        Fallible<Fallible<int>> outer = inner;
        
        Fallible<int> result = outer;

        Assert.Equal(expectedError, result.Error);
    }

    [Fact]
    public void WhenImplicitlyUnwrapped_ShouldContainError_WhenOuterErrorIsPresent()
    {
        var expectedError = new Error("Outer Error");
        Fallible<Fallible<int>> outer = expectedError;
        
        Fallible<int> result = outer;

        Assert.Equal(expectedError, result.Error);
    }

    #endregion

    #region Static Method Tests

    [Fact]
    public void FromCall_ShouldCatchException_AndReturnError()
    {
        var func = (int arg) =>
        {
            if (arg == 42) throw new Exception();
            return arg + 3;
        };

        var result = Fallible.Try(() => func(42));

        Assert.NotNull(result.Error);
    }
    
    [Fact]
    public void FromCall_ShouldReturnValue_WhenNoException()
    {
        var func = (int arg) =>
        {
            if (arg == 42) throw new Exception();
            return arg + 3;
        };

        var (value, error) = Fallible.Try(() => func(41));

        Assert.Null(error);
        Assert.Equal(44, value);
    }
    
    [Fact]
    public void FromCall_ShouldHaveErrorMessage_ContainingExpression()
    {
        var func = (int arg) =>
        {
            if (arg == 42) throw new Exception();
            return arg + 3;
        };

        var (_, error) = Fallible.Try(() => func(42));
        
        Assert.Contains("() => func(42)", error.Message);
    }

    #endregion
}