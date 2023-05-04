using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Valuable.Fallible.Tests;

public class ErrorTests
{
    public ErrorTests()
    {
        Arb.Register<Generators>();
    }
    
    #region Instantiation Tests

    [Property]
    public Property WhenCreated_ShouldHaveMessage(string message) =>
        (new Error(message).Message == message).ToProperty();
    
    [Property]
    public Property WhenCreated_ShouldHaveStackTrace(Error error) =>
        error.StackTrace.Contains("at FallibleTypes.Error..ctor").ToProperty();
    
    #endregion

    #region Conversion Tests

    [Property]
    public Property WhenCreated_BoolConversion_ShouldReturnTrue(Error error) =>
        ((bool) error).ToProperty();

    [Fact]
    public void WhenDefault_BoolConversion_ShouldReturnFalse()
    {
        Error error = default!;
        
        Assert.False(error);
    }

    #endregion

    #region Equality Tests

    [Property]
    public Property WhenEquated_GivenSameInstance_ShouldReturnTrue(Error error) =>
        error.Equals(error).ToProperty();

    [Property]
    public Property WhenEquated_GivenDifferentInstances_ShouldReturnFalse(Error error1, Error error2) => 
        (!error1.Equals(error2)).When(error1.Message != error2.Message);

    [Property]
    public Property WhenEquated_GivenNull_ShouldReturnFalse(Error error) =>
        (!error.Equals(null)).ToProperty();

    [Property]
    public Property WhenEquated_GivenDifferentType_ShouldReturnFalse(Error error, object obj) =>
        (!error.Equals(obj)).When(obj is not Error);

    [Fact]
    public void WhenEquated_GivenSameMessages_DifferentLocationOfLocation_ShouldReturnFalse()
    {
        var error = new Error("Test");
        var sameError = new Error("Test");
        
        var result = error.Equals(sameError);
        
        Assert.False(result);
    }
    
    [Fact]
    public void WhenEquated_GivenSameLocationOfInstantiation_ShouldReturnTrue()
    {
        Error CreateError() => new("Test");
        var error1 = CreateError();
        var error2 = CreateError();
        
        var result = error1.Equals(error2);
        
        Assert.True(result);
    }

    #endregion

    #region ToString Tests

    [Property]
    public Property ToString_ContainsMessage(Error error) => 
        error.ToString().Contains(error.Message).ToProperty();

    [Property]
    public Property ToString_ContainsStackTrace(Error error) => 
        error.ToString().Contains("at FallibleTypes.Error..ctor").ToProperty();

    #endregion

    #region Message Tests

    [Fact]
    public void Format_FormatsMessageUsingStringFormat()
    {
        const string expected = "test :test: test";
        var error = new Error("test");
        
        error.Format("test :{0}: test", error.Message);
        
        Assert.Equal(expected, error.Message);
    }

    [Property]
    public Property AdditionOperator_DoesNotChangeExistingMessage_WhenStringOnLHS(Error error)
    {
        var originalMessage = error.Message;
        return ("" + error).Message.Equals(originalMessage).ToProperty();
    }
    
    [Property]
    public Property AdditionOperator_DoesNotChangeExistingMessage_WhenStringOnRHS(Error error)
    {
        var originalMessage = error.Message;
        return (error + "").Message.Equals(originalMessage).ToProperty();
    }

    [Property]
    public Property AdditionOperator_PrependsString_WhenStringOnLHS(NonNull<string> message, Error error) =>
        (message.Get + error).Message.StartsWith(message.Get).ToProperty();

    [Property]
    public Property AdditionOperator_AppendsString_WhenStringOnRHS(NonNull<string> message, Error error) =>
        (error + message.Get).Message.EndsWith(message.Get).ToProperty();

    #endregion
}