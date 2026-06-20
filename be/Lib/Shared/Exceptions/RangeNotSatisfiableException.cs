namespace Lib.Shared.Exceptions;

[Serializable]
public class RangeNotSatisfiableException : Exception
{
    public RangeNotSatisfiableException()
    {
    }

    public RangeNotSatisfiableException(string message) : base(message)
    {
    }

    public RangeNotSatisfiableException(string message, Exception inner) : base(message, inner)
    {
    }
}