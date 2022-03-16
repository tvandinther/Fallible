using System.Runtime.CompilerServices;
using System.Text;

namespace Fallible;

public class Error : IEquatable<Error>
{
    public string Message { get; private set; }
    public readonly string StackTrace;
    private readonly string _callingFilePath;
    private readonly string _callingMemberName;
    private readonly int _callingLineNumber;

    public Error(string message, [CallerFilePath] string callingFilePath = "",
        [CallerMemberName] string callingMemberName = "", [CallerLineNumber] int callingSourceLineNumber = 0)
    {
        Message = message;
        StackTrace = Environment.StackTrace;
        _callingFilePath = callingFilePath;
        _callingMemberName = callingMemberName;
        _callingLineNumber = callingSourceLineNumber;
    }

    public static implicit operator bool(Error? error) => error is not default(Error);
    public static Error operator +(string message, Error error)
    {
        error.Message = string.Concat(message, error.Message);
        return error;
    }
    
    public static Error operator +(Error error, string message)
    {
        error.Message = string.Concat(error.Message, message);
        return error;
    }

    public void Format(string format, params object[] args)
    {
        Message = string.Format(format, args);
    }

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