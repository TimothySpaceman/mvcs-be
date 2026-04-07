using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.DTOs;

public record DeviceAuthTransactionDto
{
    public string UserCode { get; init; }
    public string DeviceCode { get; init; }
    public DeviceInfo DeviceInfo { get; init; }
    public string IpAddress { get; init; }
    public Guid? UserId { get; private set; }
    public bool IsConfirmed { get; private set; }
    public bool IsAborted { get; private set; }

    public DeviceAuthTransactionDto(
        string userCode,
        string deviceCode,
        DeviceInfo deviceInfo,
        string ipAddress,
        Guid? userId,
        bool isConfirmed,
        bool isAborted
    )
    {
        UserCode = userCode;
        DeviceCode = deviceCode;
        DeviceInfo = deviceInfo;
        IpAddress = ipAddress;
        UserId = userId;
        IsConfirmed = isConfirmed;
        IsAborted = isAborted;
    }

    public void Confirm(Guid userId)
    {
        UserId = userId;
        IsConfirmed = true;
    }

    public void Abort()
    {
        IsAborted = true;
    }
};