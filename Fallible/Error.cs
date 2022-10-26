using System.Runtime.CompilerServices;
using System.Text;

namespace FallibleTypes;


/// <summary>
/// Represents a failed state.
/// </summary>
public class Error : IEquatable<Error>
{
    /// <summary>
    /// A user-friendly error message.
    /// </summary>
    public string Message { get; private set; }
    
    /// <summary>
    /// The stack trace of the error.
    /// </summary>
    public readonly string StackTrace;
    
    private readonly string _callingFilePath;
    private readonly string _callingMemberName;
    private readonly int _callingLineNumber;

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> class.
    /// </summary>
    /// <param name="message">A user-friendly error message.</param>
    public Error(string message = "", [CallerFilePath] string callingFilePath = "",
        [CallerMemberName] string callingMemberName = "", [CallerLineNumber] int callingSourceLineNumber = 0)
    {
        Message = message;
        StackTrace = Environment.StackTrace;
        _callingFilePath = callingFilePath;
        _callingMemberName = callingMemberName;
        _callingLineNumber = callingSourceLineNumber;
    }

    public static implicit operator bool(Error? error) => error is not default(Error);
    
    /// <summary>
    /// Prepends a message to the error message.
    /// </summary>
    /// <param name="message">The message to prepend.</param>
    /// <param name="error">The error to which the message is being prepended.</param>
    /// <returns>The same error with a concatenated error message.</returns>
    public static Error operator +(string message, Error error)
    {
        error.Message = string.Concat(message, error.Message);
        return error;
    }
    
    /// <summary>
    /// Appends a message to the error message.
    /// </summary>
    /// <param name="message">The message to append.</param>
    /// <param name="error">The error to which the message is being appended.</param>
    /// <returns>The same error with a concatenated error message.</returns>
    public static Error operator +(Error error, string message)
    {
        error.Message = string.Concat(error.Message, message);
        return error;
    }

    /// <summary>
    /// Formats the error message using the specified format string.
    /// </summary>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    /// <remarks>Uses <see cref="string"/>.Format in the implementation.</remarks>
    public void Format(string format, params object[] args)
    {
        Message = string.Format(format, args);
    }

    /// <summary>
    /// Checks error equality.
    /// </summary>
    /// <param name="other">The object being compared.</param>
    /// <returns>A boolean.</returns>
    /// <remarks>Equality is checked by the <see cref="Message"/>, <see cref="_callingFilePath"/>,
    /// <see cref="_callingMemberName"/> and <see cref="_callingLineNumber"/> properties. In combination, these properties
    /// intend to represent a specific error condition within the application.</remarks>
    public bool Equals(Error? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return
            _callingLineNumber == other._callingLineNumber
            && Message == other.Message
            && _callingMemberName == other._callingMemberName
            && _callingFilePath == other._callingFilePath;


    }

    /// <inheritdoc cref="Equals(FallibleTypes.Error?)"/>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Error) obj);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Message, _callingFilePath, _callingMemberName, _callingLineNumber);
    }
    
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(Message);
        stringBuilder.AppendLine(StackTrace);

        return stringBuilder.ToString();
    }
}