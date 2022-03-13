# Fallible

##### Author: Tom van Dinther

An idiomatic way to explicitly define, propagate and handle error states in C#. This library is inspired by Go's [errors](https://gobyexample.com/errors).

The purpose of this library is to support the usage of a new pattern of error propagation in C#. Instead of throwing exceptions and implicitly requiring callers to catch them, the pattern used in this library explicitly defines possibilities of error states in the return type and expects them to the caller to consciously address it.

The benefit of this approach is that it enforces a higher level of care in the code that consumes services and methods which can introduce error state into an application. Errors can either be handled by the caller or be passed up the stack explicitly.

### Without Fallible
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
### With Fallible
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

## Usage

The library includes two main types: `Error` and `Fallible<T>`. `Error` is a type that represents an error state while `Fallible<T>` is a type that represents a value that can either be in an error state or a success state.

### `Error`

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

### `Fallible<T>`

`Fallible<T>` is a readonly record struct meaning that it is immutable, is identified by its properties and is allocated on the stack. It can not be created directly. To create a `Fallible<T>` object you can cast either explicitly or implicitly from one of the following three types.

- `T`
- `Error`
- `(T?, Error?)`

This gives the succinct interface for the creation of a `Fallible<T>` object as per the examples below.

```c#
public Fallible<int> GetValue(int arg)
{
    if (arg == 42) return new Error("Can't work with 42");
    
    return arg + 3;
}
```
or
```c#
public Fallible<int> GetValue(int arg)
{
    if (arg == 42) return (default, new Error("Can't work with 42"));
    
    return (arg + 3, null);
}
```

#### Returning `void`

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

### Making calls to exception throwing methods

It is important to keep in mind that even if you are using this library throughout your entire application, external dependencies may not and exceptions could still be thrown when the call stack leaves your domain. Fallible includes a handy static factory method to wrap the call into a closure and have it catch any exceptions and convert them into an `Error` object.

For example, `DateTime.Parse` can throw two exceptions: `FormatException` and `ArgumentNullException`. To intercept these exceptions and convert them into an `Error` object, you can do the following:

```c#
var (result, error) = Fallible.Try(() => DateTime.Parse("1/1/2019"));
```

## Final Notes

If you are using this library in your project, it does not mean that you can not use exceptions. Exceptions are still an effective way of quickly returning up the call stack when the application is in a serious erroneous state. This usage would be similar to `panic()` in Go. I hope you enjoy using this library and find it an enjoyable addition to the C# coding experience.

### Future Work

#### Error Handling

I am considering adding in utility classes to make it easier to handle error states and reduce the amount of boilerplate that the pattern creates. Although it is already minimal, some usages of the library may show the need for these classes.

#### Error Aggregation

While typically an application will exit early upon encountering an error state, it could sometimes be beneficial to continue processing and aggregate all the error states into a single error state. This could be useful for example if you are validating a series of values and you want to collect everything wrong about a particular call before exiting.

#### Extensions to the Standard Library

Adding extension methods to the standard library is a potential improvement for the library. For example, `bool Try(out value)` type methods could be extended to support `Fallible<T> Try()` signatures.
