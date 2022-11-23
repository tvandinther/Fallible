using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace FallibleTypes.Tests;

public class FallibleTests
{
    public FallibleTests()
    {
        Arb.Register<Generators>();
    }
    
    #region Instantiation and Conversion Tests

    [Property]
    public Property Instantiation_ShouldProvideValue_WhenSuccessful(int value)
    {
        Fallible<int> fallible = value;
        return (fallible.Value == value).ToProperty();
    }
    
    [Property]
    public Property Instantiation_ShouldProvideDefaultValue_WhenFailed(Error error)
    {
        Fallible<int> fallible = error;
        return (fallible.Value == default).ToProperty();
    }
    
    [Property]
    public Property Instantiation_ShouldProvideDefaultError_WhenSuccessful(int value)
    {
        Fallible<int> fallible = value;
        return (fallible.Error == default).ToProperty();
    }
    
    [Property]
    public Property Instantiation_ShouldProvideError_WhenFailed(Error error)
    {
        Fallible<int> fallible = error;
        return fallible.Error.Equals(error).ToProperty();
    }

    [Property]
    public Property ShouldImplicitlyUnwrap_WhenDoubleWrapped(Fallible<int> fallible)
    {
        Fallible<Fallible<int>> wrapped = fallible;
        Fallible<int> unwrapped = wrapped;
        return unwrapped.Equals(fallible).ToProperty();
    }

    #endregion

    #region Deconstruction Tests

    [Property]
    public Property ValueIsFirst_WhenDeconstructed(Fallible<int> fallible)
    {
        var (value, _) = fallible;
        return (value == fallible.Value).ToProperty();
    }
    
    [Property]
    public Property ErrorIsSecond_WhenDeconstructed(Fallible<int> fallible)
    {
        var (_, error) = fallible;
        return (error == fallible.Error).ToProperty();
    }

    #endregion

    #region Conversion Tests

    #region Static Tests

    /*
     * Tests are expected to pass if they compile.
     * This code serves to fail a compilation if type rules are altered.
     */
    
    [Property]
    public Property ImplicitlyConvertable_FromValue(int value)
    {
        Fallible<int> fallible = value;
        return (fallible is Fallible<int>).ToProperty();
    }
    
    [Property]
    public Property ImplicitlyConvertable_FromError(Error error)
    {
        Fallible<int> fallible = error;
        return (fallible is Fallible<int>).ToProperty();
    }
    
    [Property]
    public Property FallibleReturn_ReturnsCustomVoidType()
    {
        var fallible = Fallible.Return;
        return (fallible is Fallible<Void>).ToProperty();
    }

    [Property]
    public Property ImplicitlyUnwrapsItself(int value)
    {
        Fallible<int> inner = value;
        Fallible<Fallible<int>> outer = inner;
        Fallible<int> result = outer;
        return (result is Fallible<int>).ToProperty();
    }

    #endregion
    
    [Property]
    public Property UnwrappedFallible_ShouldContainInnerValue(int value)
    {
        Fallible<int> inner = value;
        Fallible<Fallible<int>> outer = inner;
        Fallible<int> result = outer;
        return (result.Value == value).ToProperty();
    }
    
    [Property]
    public Property UnwrappedFallible_ShouldContainInnerError(Error error)
    {
        Fallible<int> inner = error;
        Fallible<Fallible<int>> outer = inner;
        Fallible<int> result = outer;
        return result.Error.Equals(error).ToProperty();
    }

    #endregion

    [Property]
    public Property Void()
    {
        var (result, _) = Fallible.Return;
        
        return (result.GetType() == typeof(Void)).ToProperty();
    }
}