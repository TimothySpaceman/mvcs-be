using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Entities;

namespace Lib.Modules.Auth.Services;

public interface IDeviceAuthService
{
    public Task<DeviceAuthTransactionDto> StartTransactionAsync(DeviceWithIpDto deviceDto);
    public Task<DeviceAuthTransactionDto?> GetByUserCodeAsync(string userCode);
    public Task<DeviceAuthTransactionDto?> GetByDeviceCodeAsync(string deviceCode);
    public Task ConfirmByUserCodeAsync(string userCode, Guid userId);
    public Task AbortByUserCodeAsync(string userCode);
    public Task CloseByDeviceCodeAsync(string deviceCode);
}