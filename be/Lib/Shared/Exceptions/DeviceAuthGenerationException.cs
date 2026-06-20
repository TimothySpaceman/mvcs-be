namespace Lib.Shared.Exceptions;

[Serializable]
public class DeviceAuthGenerationException : Exception
{
    public DeviceAuthGenerationException()
    {
    }

    public DeviceAuthGenerationException(string message) : base(message)
    {
    }

    public DeviceAuthGenerationException(string message, Exception inner) : base(message, inner)
    {
    }
}