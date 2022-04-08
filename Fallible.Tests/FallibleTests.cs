// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Fallible.Extensions;
// using Fallible.Extensions.Continuation;
// using Xunit;
//
// namespace FallibleTypes.Tests;
//
// public class FallibleTests
// {
//     #region Instantiation Tests
//
//     [Fact]
//     public void WhenCreated_ContainsError()
//     {
//         var error = new Error("Wrong Number");
//
//         Fallible<int> fallible = error;
//         
//         Assert.Equal(error, fallible.Error);
//     }
//
//     [Fact]
//     public void WhenCreated_ContainsDefaultValue_WhenValueType()
//     {
//         var error = new Error("Wrong Number");
//
//         Fallible<int> fallible = error;
//         
//         Assert.Equal(default, fallible.Value);
//     }
//     
//     [Fact]
//     public void WhenCreated_ContainsDefaultValue_WhenReferenceType()
//     {
//         var error = new Error("Wrong Number");
//
//         Fallible<object> fallible = error;
//
//         Assert.Equal(default, fallible.Value);
//         Assert.Null(fallible.Value);
//     }
//
//     #endregion
//
//     #region Deconstruction Tests
//
//     [Fact]
//     public void CanBeDeconstructed_ValueIsDefault()
//     {
//         var expectedError = new Error("Wrong Number");
//         Fallible<int> fallible = expectedError;
//         
//         var (number, _) = fallible;
//         
//         Assert.Equal(default, number);
//     }
//     
//     [Fact]
//     public void CanBeDeconstructed_ErrorIsExpected()
//     {
//         var expectedError = new Error("Wrong Number");
//         Fallible<int> fallible = expectedError;
//         
//         var (_, error) = fallible;
//         
//         Assert.Equal(expectedError, error);
//     }
//     
//     [Fact]
//     public void CanBeDeconstructed_ValueIsExpected()
//     {
//         var expectedValue = 42;
//         Fallible<int> fallible = expectedValue;
//
//         var (value, _) = fallible;
//         
//         Assert.Equal(expectedValue, value);
//     }
//
//     [Fact]
//     public void CanBeDeconstructed_ShouldHaveNullResult_WhenVoidReturn()
//     {
//         Fallible<Void> fallible = new Error("Wrong Number");
//         
//         var (result, _) = fallible;
//         
//         Assert.Null(result);
//     }
//
//     #endregion
//
//     #region Conversion Tests
//
//     [Fact]
//     public void CanBeImplicitlyConverted_FromValue()
//     {
//         Fallible<int> fallible = 42;
//         
//         Assert.IsType<Fallible<int>>(fallible);
//     }
//     
//     [Fact]
//     public void CanBeImplicitlyConverted_FromError()
//     {
//         Fallible<int> fallible = new Error("Test");
//         
//         Assert.IsType<Fallible<int>>(fallible);
//     }
//
//     [Fact]
//     public void CanBeImplicitlyConverted_FromVoid_WhenReturning()
//     {
//         var func = Fallible<Void>() => global::Fallible.Extensions.Fallible.Return;
//
//         Fallible<Void> fallible = func();
//         
//         Assert.IsType<Fallible<Void>>(fallible);
//
//     }
//     
//     [Fact]
//     public void WhenWrapped_ShouldUnwrapItselfImplicitly()
//     {
//         Fallible<int> inner = 42;
//         Fallible<Fallible<int>> outer = inner;
//         
//         Fallible<int> result = outer;
//
//         Assert.IsType<Fallible<int>>(result);
//     }
//     
//     [Fact]
//     public void WhenImplicitlyUnwrapped_ShouldContainInnerValue()
//     {
//         const int expectedValue = 42;
//         Fallible<int> inner = expectedValue;
//         Fallible<Fallible<int>> outer = inner;
//         
//         Fallible<int> result = outer;
//
//         Assert.Equal(expectedValue, result.Value);
//     }
//
//     [Fact]
//     public void WhenImplicitlyUnwrapped_ShouldContainCorrectError_WhenInnerErrorIsPresent()
//     {
//         var expectedError = new Error("Inner Error");
//         Fallible<int> inner = expectedError;
//         Fallible<Fallible<int>> outer = inner;
//         
//         Fallible<int> result = outer;
//
//         Assert.Equal(expectedError, result.Error);
//     }
//
//     [Fact]
//     public void WhenImplicitlyUnwrapped_ShouldContainError_WhenOuterErrorIsPresent()
//     {
//         var expectedError = new Error("Outer Error");
//         Fallible<Fallible<int>> outer = expectedError;
//         
//         Fallible<int> result = outer;
//
//         Assert.Equal(expectedError, result.Error);
//     }
//
//     #endregion
//
//     #region Static Method Tests
//
//     [Fact]
//     public void FromCall_ShouldCatchException_AndReturnError()
//     {
//         var func = (int arg) =>
//         {
//             if (arg == 42) throw new Exception();
//             return arg + 3;
//         };
//
//         var result = global::Fallible.Extensions.Fallible.Try(() => func(42));
//
//         Assert.NotNull(result.Error);
//     }
//     
//     [Fact]
//     public void FromCall_ShouldReturnValue_WhenNoException()
//     {
//         var func = (int arg) =>
//         {
//             if (arg == 42) throw new Exception();
//             return arg + 3;
//         };
//
//         var (value, error) = global::Fallible.Extensions.Fallible.Try(() => func(41));
//
//         Assert.Null(error);
//         Assert.Equal(44, value);
//     }
//     
//     [Fact]
//     public void FromCall_ShouldHaveErrorMessage_ContainingExpression()
//     {
//         var func = (int arg) =>
//         {
//             if (arg == 42) throw new Exception();
//             return arg + 3;
//         };
//
//         var (_, error) = global::Fallible.Extensions.Fallible.Try(() => func(42));
//         
//         Assert.Contains("() => func(42)", error.Message);
//     }
//
//     #endregion
//
//     #region Fluent Errors Tests
//
//     [Fact]
//     public void Try_ReturnsValue_WhenOperationSucceeds()
//     {
//         const int expectedValue = 42;
//         
//         var (value, _) = global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(expectedValue, false));
//         
//         Assert.Equal(expectedValue, value);
//     }
//
//     [Fact]
//     public void Try_ReturnsError_WhenOperationFails()
//     {
//         var (_, error) = global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true));
//         
//         Assert.NotNull(error);
//     }
//     
//     [Fact]
//     public void Try_PrependsErrorMessage_WhenOperationFails()
//     {
//         const string expectedStartString = "Test Error: ";
//         
//         var (_, error) = global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true), expectedStartString);
//         
//         Assert.StartsWith(expectedStartString, error.Message);
//     }
//     
//     [Fact]
//     public void Then_ReturnsValue_WhenOperationSucceeds()
//     {
//         const int expectedValue = 42;
//         
//         var result = FallibleOperation(expectedValue, false)
//             .Then(value => value + 3);
//         
//         Assert.Equal(45, result);
//     }
//     
//     [Fact]
//     public void Then_ReturnsError_WhenOperationFails()
//     {
//         var result = FallibleOperation(42, true)
//             .Then(value => value + 3);
//         
//         Assert.NotNull(result.Error);
//     }
//
//     [Fact]
//     public void OnFail_ReturnsPassesThroughFallible_WhenOperationSucceeds_ErrorReturningOnFail()
//     {
//         const int expectedValue = 42;
//         
//         var result = global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(expectedValue, false)), error => error);
//         
//         Assert.Equal(expectedValue, result.Value);
//     }
//     
//     [Fact]
//     public void OnFail_ReturnsPassesThroughFallible_WhenOperationSucceeds_TransparentOnFail()
//     {
//         const int expectedValue = 42;
//         var callCount = 0;
//         
//         var result = global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(expectedValue, false)), () => callCount++);
//         
//         Assert.Equal(0, callCount);
//         Assert.Equal(expectedValue, result.Value);
//     }
//     
//     [Fact]
//     public void OnFail_ReturnsModifiedError_WhenOperationFails_ErrorReturningOnFail()
//     {
//         const string expectedStartString = "Test Error: ";
//         
//         var result = global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true)), error => expectedStartString + error);
//         
//         Assert.StartsWith(expectedStartString, result.Error.Message);
//     }
//     
//     [Fact]
//     public void OnFail_ReturnsPassesThroughFallible_WhenOperationFails_TransparentOnFail()
//     {
//         var callCount = 0;
//         
//         var result = global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true)), () => callCount++);
//         
//         Assert.Equal(1, callCount);
//         Assert.NotNull(result.Error);
//     }
//     
//     [Fact]
//     public void OnFail_CallsOnFailFunc_WhenOperationFails_TransparentOnFail()
//     {
//         var callCount = 0;
//
//         global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true)), _ => callCount++);
//         
//         Assert.Equal(1, callCount);
//     }
//     
//     [Fact]
//     public void OnFail_ReturnsError_WhenOperationFails_ErrorReturningOnFail()
//     {
//         var (_, error) = global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true)), e => e);
//         
//         Assert.NotNull(error);
//     }
//     
//     [Fact]
//     public void OnFail_ReturnsError_WhenOperationFails_TransparentOnFail()
//     {
//         var callCount = 0;
//         
//         var (_, error) = global::Fallible.Extensions.Fallible.OnFail<int>(global::Fallible.Extensions.Fallible.Try(() => FallibleOperation(42, true)), () => callCount++);
//         
//         Assert.Equal(1, callCount);
//         Assert.NotNull(error);
//     }
//
//     [Fact]
//     public void CanChainFluently_Succeeds()
//     {
//         var result = FallibleOperation(42, false)
//             .Then(value => value + 3)
//             .OnFail(error => error);
//         
//         Assert.Equal(45, result.Value);
//     }
//     
//     [Fact]
//     public void CanChainFluently_Fails()
//     {
//         var callCount = 0;
//         
//         var result = FallibleOperation(42, false)
//             .Then(value => FallibleOperation(value + 3, true))
//             .OnFail(_ => callCount++);
//         
//         Assert.Equal(1, callCount);
//     }
//     
//     [Fact]
//     public void CanChainFluently_Fails_DoesNotExecuteThen()
//     {
//         var thenCallCount = 0;
//
//         FallibleOperation(42, true)
//             .Then(_ => FallibleOperation(thenCallCount++, true));
//         
//         Assert.Equal(0, thenCallCount);
//     }
//     
//     [Fact]
//     public void CanChainNested_WhenRecovered()
//     {
//         const int expectedValue = 42;
//         
//         var (result, _) = FallibleOperation(0, true)
//             .Then(value => value + 1)
//             .Recover(_ => 
//                 FallibleOperation(expectedValue, false)
//                     .Then(_ => expectedValue + 1)
//                     .OnFail(error => "Could not get players: " + error));
//         
//         Assert.Equal(expectedValue + 1, result);
//     }
//     
//     private Fallible<T> FallibleOperation<T>(T expectedValue, bool fail)
//     {
//         if (fail) return new Error("Operation Failed");
//         return expectedValue;
//     }
//
//     #endregion
//
//     #region Covariance and Contravariance
//
//     [Fact]
//     public void ToCovariant_ResolvesCovariantAssignment()
//     {
//         Fallible<List<int>> fallible = new List<int> { 42 };
//         
//         Fallible<IEnumerable<int>> covariant = fallible.ToCovariant<List<int>, IEnumerable<int>>();
//
//         Assert.IsAssignableFrom<List<int>>(covariant.Value);
//     }
//     
//     [Fact]
//     public void ToContravariant_ResolvesContravariantAssignment()
//     {
//         Fallible<IEnumerable<int>> fallible = new List<int> { 42 };
//         
//         Fallible<List<int>> contravariant = fallible.ToContravariant<IEnumerable<int>, List<int>>();
//
//         Assert.IsAssignableFrom<IEnumerable<int>>(contravariant.Value);
//     }
//
//     #endregion
//
//     #region Logical Chaining
//
//     private Fallible<int> WillFail() => new Error("Failed");
//     private Fallible<int> WillSucceed() => 42;
//     
//     [Fact]
//     public void Recover_ChainsFailedFallibles()
//     {
//         const int expectedValue = 2;
//
//         var (value, error) = global::Fallible.Extensions.Fallible.Recover(global::Fallible.Extensions.Fallible.If(WillFail), () => FallibleOperation(expectedValue, false));
//
//         Assert.Equal(expectedValue, value);
//     }
//     
//     [Fact]
//     public void Or_ReturnsFirstSuccessfulFallible()
//     {
//         const int expectedValue = 2;
//
//         var (value, error) = global::Fallible.Extensions.Fallible.OnFail(global::Fallible.Extensions.Fallible.If(() => FallibleOperation(expectedValue, false)), () => FallibleOperation(2, true));
//
//         Assert.Equal(expectedValue, value);
//     }
//     
//     [Fact]
//     public void And_ReturnsFirstFailedFallible()
//     {
//         var callCount = 0;
//
//         var (_, error) = global::Fallible.Extensions.Fallible.Then<int, int>(global::Fallible.Extensions.Fallible.If(() => FallibleOperation(callCount++, true)), () => FallibleOperation(callCount++, false));
//
//         Assert.True(error);
//         Assert.Equal(1, callCount);
//     }
//     
//     [Fact]
//     public void And_ExecutesBothFallibleOperations()
//     {
//         var callCount = 0;
//
//         global::Fallible.Extensions.Fallible.Then<int, int>(global::Fallible.Extensions.Fallible.If(() => FallibleOperation(callCount++, false)), () => FallibleOperation(callCount++, false));
//
//         Assert.Equal(2, callCount);
//     }
//
//     [Fact]
//     public void And_CanChainDifferentFallibleTypes()
//     {
//         global::Fallible.Extensions.Fallible.Then<int, string>(global::Fallible.Extensions.Fallible.If(() => FallibleOperation(2, false)), () => FallibleOperation("3", false));
//     }
//     
//     [Fact]
//     public void If_BooleanOverload_ReturnsNoError_WhenTrue()
//     {
//         var (value, error) = global::Fallible.Extensions.Fallible.Then<Void, int>(global::Fallible.Extensions.Fallible.If(true), () => 42);
//         
//         Assert.Equal(42, value);
//         Assert.False(error);
//     }
//     
//     [Fact]
//     public void If_BooleanOverload_ReturnsError_WhenFalse()
//     {
//         var (_, error) = global::Fallible.Extensions.Fallible.Then<Void, int>(global::Fallible.Extensions.Fallible.If(false), () => 42);
//         
//         Assert.True(error);
//     }
//     
//     [Fact]
//     public void AndIf_ReturnsNoError_WhenTrue()
//     {
//         const int expectedValue = 42;
//         
//         var (value, error) = global::Fallible.Extensions.Fallible.ThenIf<int>(global::Fallible.Extensions.Fallible.
//                 If(() => FallibleOperation(expectedValue, false)), x => x == expectedValue);
//         
//         Assert.Equal(expectedValue, value);
//         Assert.False(error);
//     }
//     
//     [Fact]
//     public void AndIf_ReturnsError_WhenFalse()
//     {
//         var (_, error) = global::Fallible.Extensions.Fallible.ThenIf<int>(global::Fallible.Extensions.Fallible.
//                 If(WillSucceed), x => x == x + 2);
//         
//         Assert.True(error);
//     }
//     
//     [Fact]
//     public void AndIf_ReturnsError_WhenChainedOnError()
//     {
//         var (_, error) = global::Fallible.Extensions.Fallible.ThenIf<int>(global::Fallible.Extensions.Fallible.
//                 If(WillFail), x => x == x + 2);
//         
//         Assert.True(error);
//     }
//     
//     [Fact]
//     public void OrIf_ReturnsNoError_WhenTrue()
//     {
//         var callCount = 0;
//         
//         var (_, error) = global::Fallible.Extensions.Fallible.OnFailIf<int>(global::Fallible.Extensions.Fallible.
//                 If(WillFail), true)
//             .Then(_ => callCount++);
//         
//         Assert.Equal(1, callCount);
//         Assert.False(error);
//     }
//     
//     [Fact]
//     public void OrIf_ReturnsError_WhenFalse()
//     {
//         var callCount = 0;
//         
//         var (_, error) = global::Fallible.Extensions.Fallible.OnFailIf<int>(global::Fallible.Extensions.Fallible.
//                 If(WillFail), false)
//             .Then(_ => callCount++);
//         
//         Assert.Equal(0, callCount);
//         Assert.True(error);
//     }
//     
//     [Fact]
//     public void OrIf_ReturnsError_WhenChainedOnError()
//     {
//         var (_, error) = global::Fallible.Extensions.Fallible.OnFailIf<int>(global::Fallible.Extensions.Fallible.
//                 If(WillFail), false);
//         
//         Assert.True(error);
//     }
//
//     #endregion
//
//     #region New Logical Chaining
//
//     private Fallible<int> WillFail(int x) => new Error("Failed");
//     private Fallible<int> WillSucceed(int x) => x;
//     
//     [Fact]
//     public void CanCaptureValue_AndLinkFalliblesLogically_UsingOr()
//     {
//         var callCount = 0;
//         
//         var (result, error) = global::Fallible.Extensions.Fallible.About(42)
//             .If(WillFail)
//             .Or(WillFail)
//             .Or(WillSucceed) // Chain exits here
//             .Or(WillFail)
//             .Or(_ => callCount++) // Should not be called
//             .ContinueWith<int>(_ =>
//             {
//                 callCount++;
//                 return callCount;
//             });
//         
//         Assert.Equal(1, result);
//     }
//     
//     [Fact]
//     public void CanCaptureValue_AndLinkFalliblesLogically_UsingAndFails()
//     {
//         var callCount = 0;
//         
//         var (result, error) = global::Fallible.Extensions.Fallible.About(42)
//             .If(WillSucceed)
//             .And(WillSucceed)
//             .And(WillFail) // Chain exits here
//             .And(WillSucceed)
//             .And(_ => callCount++) // Should not be called
//             .ContinueWith<int>(_ =>
//             {
//                 callCount++;
//                 return callCount;
//             });
//         
//         Assert.Equal(0, result);
//     }
//     
//     [Fact]
//     public void CanCaptureValue_AndLinkFalliblesLogically_UsingAndSucceeds()
//     {
//         var callCount = 0;
//
//         var (result, error) = global::Fallible.Extensions.Fallible.About(42)
//             .If(WillSucceed)
//             .And(WillSucceed)
//             .And(WillSucceed)
//             .ContinueWith<int>(x =>
//             {
//                 callCount++;
//                 return callCount;
//             });
//         
//         Assert.Equal(1, result);
//     }
//     
//     private Fallible<string> MakeString(int x) => x.ToString();
//     
//     #endregion
// }