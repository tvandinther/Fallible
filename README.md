# Fallible

[![Tests](https://github.com/tvandinther/fallible/actions/workflows/test.yml/badge.svg)](https://www.github.com/tvandinther/fallible)
[![Nuget](https://img.shields.io/nuget/v/Fallible?color=blue)](https://www.nuget.org/packages/Fallible/)
[![Nuget](https://img.shields.io/nuget/dt/Fallible)](https://www.nuget.org/packages/Fallible/)
[![.NET](https://img.shields.io/badge/.NET-6.0-blue)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
[![GitHub](https://img.shields.io/github/license/tvandinther/fallible?color=lightgrey)](https://github.com/tvandinther/Fallible/blob/master/LICENSE)

**Author: Tom van Dinther**

An idiomatic way to explicitly define, propagate and handle error states in C#. This library is inspired by Go's [errors](https://gobyexample.com/errors).

*`Fallible<T>` is a functor. It has a "return" and a "bind" `Fallible.Map()`/`ToFallible()` extension method to bring values and functions into the fallible world.*

The purpose of this library is to support the usage of a new pattern of error propagation in C#. Instead of throwing exceptions and implicitly requiring callers to catch them, the pattern used in this library explicitly defines possibilities of error states in the return type and expects them to the caller to consciously address it.

The benefit of this approach is that it enforces a higher level of care in the code that consumes services and methods which can introduce error state into an application. Errors can either be handled by the caller or be passed up the stack explicitly.

## Without Fallible
```c#
public int GetValue(int arg)
{
    if (arg == 42) throw new Exception("Can't work with 42");
    
    return arg + 3;
}
```
The caller must know to handle the error state.
```c#
try
{
    var result = GetValue(42);
}
catch (Exception ex)
{
    // Handle error
}
// continue with result
```
## With Fallible

```c#
public Fallible<int> GetValue(int arg)
{
    if (arg == 42) return new Error("Can't work with 42");
    
    return arg + 3;
}
```
The caller is forced to acknowledge the error state.
```c#
var (result, error) = GetValue(42);
if (error)
{
    // Handle error
}
// continue with result
```

# Usage

The library includes two main types: `Error` and `Fallible<T>`. `Error` is a type that represents an error state while `Fallible<T>` is a type that represents a value that can either be in an error state or a success state.

## `Error`

`Error` is a reference type and can be created by instantiating it with a message. The message is a string which describes the error.

```c#
var error = new Error("Something went wrong");
```
The `Error` object contains two public properties that can be used to access the message and the stack trace.
```c#
string error.Message
string error.StackTrace
```

Equality and hash codes for `Error` objects are determined by where in the program the error was created and the message. This means that, for example, if a method was called from two different parts of the program, the error will still be considered equal despite containing different stack traces.

Although, if the error's message was constructed using dynamic arguments, the errors will not be considered equal if the arguments are different due to the equality check on the message property.

## `Fallible<T>`

`Fallible<T>` is a readonly record struct meaning that it is immutable, is identified by its properties and is allocated on the stack. It can not be created directly. To create a `Fallible<T>` object you can cast either explicitly or implicitly from `T` or `Error`. Using a cast from only these two types as the only way to create a `Fallible<T>` object ensures that we always have either an error or a value.

This gives the succinct interface for the creation of a `Fallible<T>` object as per the examples below.

```c#
public Fallible<int> GetValue(int arg)
{
    if (arg == 42) return new Error("Can't work with 42");
    
    return arg + 3;
}
```

### Returning `void`

Fallible includes a `Void` type that can be used to return *void* from a method. It does not have an accessible constructor and can only be created by using the `Fallible.Return` property.

```c#
public Fallible<Void> DoSomething()
{
    // Do something
    if (somethingFailed) return new Error("Something went wrong");
    
    return Fallible.Return;
}
```

### Working with `Fallible<T>`

When working with a `Fallible<T>` type returned by a method, it is best to deconstruct it upon assignment and then perform a check on the error state. `Error` contains an implicit boolean conversion operator that returns `true` if the error state is not `null`.

```c#
var (result, error) = GetValue(42);
if (error)
{
    // Handle error
}
// continue with result
```

### Implicit Unwrapping

You may want to use the `Fallible<T>` type in a wrapper function. If the wrapped function also returns a `Fallible<T>` type, an implicit conversion will automatically unwrap `Fallible<Fallible<T>>` into `Fallible<T>`.

### Making calls to exception throwing methods

It is important to keep in mind that even if you are using this library throughout your entire application, external dependencies may not and exceptions could still be thrown when the call stack leaves your domain. Fallible includes a handy static factory method to wrap the call into a closure and have it catch any exceptions and convert them into an `Error` object.

For example, `DateTime.Parse` can throw two exceptions: `FormatException` and `ArgumentNullException`. To intercept these exceptions and convert them into an `Error` object, you can do the following:

```c#
var (result, error) = Fallible.Try(() => DateTime.Parse("1/1/2019"));
```

### Concatenating error messages

When dealing with an `Error` object, often you may want to pass the error up the call stack. As it is passed up the call stack, the level of abstraction is increased which can give increasing context to the error message. To facilitate this best practice, the `Error` object allows string concatenation with itself.

```c#
Fallible<User> GetUserFromDB(UserId id)
{
    if (!databaseIsConnected) return new Error("Database is not connected");
    ...
}

Fallible<User> FindUserById(UserId id)
{
    var (user, error) = GetUserFromDB(id);
    if (error) return "Could not find user: " + error;
    
    return user;
}

var (user, error) = FindUserById(id);
if (error)
{
    Console.WriteLine(error); // "Could not find user: Database is not connected"
}
```

Messages can also be appended by putting the string on the right hand side of the `+` operator.

```c#
return error + ": Could not find user";
```

### Error message formatting

To ensure that error messages are not accidentally overwritten, directly setting the `Error.Message` property is not possible. If appending or prepending from the error message is not suitable, you can use the `Error.Format` method to format the message more comprehensively. This method functions exactly like the `string.Format` method and uses that in its implementation.

```c#
return error.Format("Could not find user: {0}: Aborting...", error.Message);
```

## Fluent Error Handling

You can use functional chaining to handle errors in a more declarative way. For example, the following code will get the user using the method with the declared signature and pass it into the next method if successful or return an `Error` object if the user is not found:

```c#
public Fallible<User> GetUserFromDB(UserId id);

return GetUserFromDB(userId).Then(user => {
    // Do something with user
});
```

This code is equivalent to the following:

```c#
public Fallible<User> GetUserFromDB(UserId id);

var (user, error) = GetUserFromDB(userId);
if (error) return error;

// Do something with user
```

You can chain multiple `Then` calls but if any of them result in a failed state, then the entire chain will fail. You can add a call to `OnFail` in the middle of the chain to handle errors.

```c#
public Fallible<User> GetUserFromDB(UserId id);

return GetUserFromDB(userId).Then(user => {
    // Do something with user
}).OnFail(error => {
    // Handle error
});
```

Multiple calls to `OnFail` can be chained together with each of them executing in turn if the chain is in a failed state.

Simply stated, `Then` and `OnFail` can be chained together as many times as necessary. When a `Then` expression is successful, the value of `Fallible<T>` is passed into the next `Then` expression. In this scenario, the chain is in a succeeded state and will not execute any expressions chained by `OnFail`. Once a `Then` expression fails, the chain will be in a failed state and any `OnFail` expressions will then be executed and finally return, skipping over `Then` expressions.

The object returned from the chain will be `Fallible<T>` where `T` is the type of the value returned by the last `Then` expression.

### `Try`

You can use the `Try` static method to wrap a call to a method that returns `Fallible<T>`. You can optionally add an error message as a parameter to the `Try` method. This will be prepend to the error message should the tried expression fail.

```c#
public Fallible<User> GetUserFromDB(UserId id);

var (user, error) = Fallible.Try(() => GetUserFromDB(userId), "Could not find user: ");

Console.WriteLine(error); // "Could not find user: Database is not connected"
```

## Fluent Logic

Fallible offers a fluent API for logic that can be used to handle `Fallible<T>` objects. `If()`, `And()` and `Or()` are all methods that return a `Fallible<T>` object for chaining. These methods are similar to `Try()`, `Then()` and `OnFail()` but they do not pass through values or errors. The latter methods could be used with discarded parameters, but the former methods enhance readability and pair nicely with extracted methods such as the following example.

### Value capturing and evaluation

```c#
Fallible.About(text).If(text => text.Length < 5)
    .Or(text => text.StartsWith("Hello"))
    .Then(text => {
    // Do something with text
});
```

### `Fallible.If()`

`Fallible.If()` lets you create a `Fallible<Void>` object from an expression. If the expression evaluates to false, the `Fallible<Void>` object will be in a failed state. If the expression evaluates to true, the `Fallible<Void>` object will be in a succeeded state.

```c#
Fallible.If(variable is null).Then(() => {
    // Do something
});
```

### `OnFail()`

`OnFail()` takes a parameterless function or a function with the error as an argument, returning a `Fallible<T>` object. The function will only be executed if the chain is in a failed state. This method is useful for reverting the state of a failed chain. The main use case for this method is to try a few different fallible operations and either return the first successful operation or finalise with a failed state.

```c#
var (value, error) = 
    FirstFallibleOperation.
    OnFail(SecondFallibleOperation).
    OnFail(ThirdFallibleOperation).
    OnFail(error => {
        // Handle error
    });
    
    // or
    //
    // OnFail(() => {
    //     // Handle error
    // });
    
Console.WriteLine(error); // Error from ThirdFallibleOperation
Console.WriteLine(value); // Value from first successful operation
```

### `OnFailIf()`

`OnFailIf()` is like `OnFail()` except that it takes an expression argument.

```c#
FirstFallibleOperation
    .OnFailIf(doTheBackupPlanFlag == true)
    .And(FallibleBackupPlan)
    .Then(() => {
    // Do something
});
```

If the expression does not read easily, it is recommended to extract it into its own method returning `Fallible<Void>`.

*Note: it is not possible to chain `OrIf()` with `Then()` or `And()` as failed chains do not propagate values. Instead use `Fallible.About()` to capture values.*

### `Then()`

`Then()` is similar to `Then()` except that it accepts a parameterless function returning a `Fallible<T>` object. The function will only be executed if the chain is in a succeeded state.

```c#
var (value, error) = 
    FirstFallibleOperation.
    Then(SecondFallibleOperation).
    Then(ThirdFallibleOperation)
    
Console.WriteLine(error); // Error from first failed operation
Console.WriteLine(value); // Value from ThirdFallibleOperation
```

### `ThenIf()`

```c#
// Provided these two functions
public Fallible<User> IfUserExists(UserId id) => Fallible.Try(() => GetUserFromDB(id), "Could not find user in database");

public bool UserIsCustomer(User user) => user.Type == UserTypes.Customer;

// You can chain logic fluently
IfUserExists(userId).ThenIf(UserIsCustomer).Then(user => {
    // Do something with user
});
```

*Note: `AndIf()` also accepts a simple expression as an argument. If succeeded, the value will be passed on.*

## Handling Covariance and Contravariance

### Covariance

To convert a `Fallible<T>` to a `Fallible<U>` where `T` is a subtype of `U`, you can use the `ToCovariant` method. This method will return a `Fallible<U>` where the value of `Fallible<U>` is the value of `Fallible<T>` converted to `U`. E.g. `Fallible<List<int>>` to `Fallible<IEnumerable<int>>`.

```c#
Fallible<List<int>> fallible = new List<int> { 42 };
Fallible<IEnumerable<int>> covariant = fallible.ToCovariant<List<int>, IEnumerable<int>>();
```

### Contravariance

To convert a `Fallible<U>` to a `Fallible<T>` where `T` is a subtype of `U`, you can use the `ToContravariant` method. This method will return a `Fallible<T>` where the value of `Fallible<T>` is the value of `Fallible<U>` converted to `T`. E.g. `Fallible<IEnumerable<int>>` to `Fallible<List<int>>`.

```c#
Fallible<IEnumerable<int>> fallible = new List<int> { 42 };
Fallible<List<int>> contravariant = fallible.ToContravariant<IEnumerable<int>, List<int>>();
```

## Final Notes

If you are using this library in your project, it does not mean that you can not use exceptions. Exceptions are still an effective way of quickly returning up the call stack when the application is in a serious erroneous state. This usage would be similar to `panic()` in Go. I hope you enjoy using this library and find it an enjoyable addition to the C# coding experience.

### Future Work

#### Error Aggregation

While typically an application will exit early upon encountering an error state, it could sometimes be beneficial to continue processing and aggregate all the error states into a single error state. This could be useful for example if you are validating a series of values and you want to collect everything wrong about a particular call before exiting.

#### Extensions to the Standard Library

I would like to add several proxy classes to encapsulate standard library functions with `Fallible<T>` return types where exceptions would otherwise be thrown. I will start with `System.Collections.Generic` as this is frequently used.
