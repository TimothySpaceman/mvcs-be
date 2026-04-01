using System.Security.Cryptography;
using Lib.Infrastructure.Redis;
using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Entities;
using Lib.Shared.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Lib.Modules.Auth.Services;

// TODO: Make multi-request operations atomic
public class DeviceAuthService(
    IRedisService redisService,
    IConfiguration config
) : IDeviceAuthService
{
    public async Task<DeviceAuthTransactionDto> StartTransactionAsync(
        DeviceWithIpDto deviceDto
    )
    {
        var transaction = await CreateTransactionDto(deviceDto);

        var ttlMinutes = config.GetValue<double>("Auth:DeviceFlow:TransactionExpiryMinutes");
        var expiration = TimeSpan.FromMinutes(ttlMinutes);

        await redisService.SetAsync(GetUserCodeKey(transaction.UserCode), transaction, expiration);
        await redisService.SetAsync(GetDeviceCodeKey(transaction.DeviceCode), transaction.UserCode, expiration);

        return transaction;
    }

    public async Task<DeviceAuthTransactionDto?> GetByUserCodeAsync(string userCode)
    {
        return await redisService.GetAsync<DeviceAuthTransactionDto>(GetUserCodeKey(userCode));
    }

    public async Task<DeviceAuthTransactionDto?> GetByDeviceCodeAsync(string deviceCode)
    {
        var userCodeKey = GetDeviceCodeKey(deviceCode);
        var userCode = await redisService.GetAsync<string>(userCodeKey);
        if (userCode is null) return null;

        var key = GetUserCodeKey(userCode);
        var transaction = await redisService.GetAsync<DeviceAuthTransactionDto>(key);
        return transaction;
    }

    public async Task ConfirmByUserCodeAsync(string userCode, Guid userId)
    {
        var key = GetUserCodeKey(userCode);

        var transaction = await redisService.GetAsync<DeviceAuthTransactionDto>(key);
        if (transaction is null)
        {
            throw new NotFoundException("No pending transaction found by this user code");
        }

        if (transaction.IsConfirmed) return;
        if (transaction.IsAborted)
        {
            throw new InvalidOperationException("Transaction is aborted");
        }

        transaction.Confirm(userId);
        await redisService.SetAsync(key, transaction);
    }

    public async Task AbortByUserCodeAsync(string userCode)
    {
        var key = GetUserCodeKey(userCode);

        var transaction = await redisService.GetAsync<DeviceAuthTransactionDto>(key);
        if (transaction is null)
        {
            throw new NotFoundException("No pending transaction found by this user code");
        }

        if (transaction.IsAborted || transaction.IsConfirmed) return;

        transaction.Abort();
        await redisService.SetAsync(key, transaction);
    }

    public async Task CloseByDeviceCodeAsync(string deviceCode)
    {
        var userCodeKey = GetDeviceCodeKey(deviceCode);
        var userCode = await redisService.GetAsync<string>(userCodeKey);
        if (userCode is null) throw new NotFoundException("No pending transaction found by this device code");

        var key = GetUserCodeKey(userCode);
        var transaction = await redisService.GetAsync<DeviceAuthTransactionDto>(key);
        if (transaction is null)
        {
            throw new NotFoundException("No pending transaction found by this device code");
        }

        await redisService.DeleteAsync(userCodeKey);
        await redisService.DeleteAsync(key);
    }

    private string GetUserCodeKey(string userCode) => $"auth:device-transaction:user-code:{userCode}";
    private string GetDeviceCodeKey(string deviceCode) => $"auth:device-transaction:device-code:{deviceCode}";

    private async Task<string> GenerateUserCode()
    {
        var attempts = 0;
        do
        {
            var userCode = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
            if (!(await redisService.ExistsAsync(GetUserCodeKey(userCode)))) return userCode;
            attempts++;
        } while (attempts < 100);

        throw new DeviceAuthGenerationException("Failed to create auth transaction");
    }

    private async Task<DeviceAuthTransactionDto> CreateTransactionDto(DeviceWithIpDto deviceDto)
    {
        var userCode = await GenerateUserCode();

        var deviceCodeBytes = RandomNumberGenerator.GetBytes(16);
        var deviceCode = Convert.ToHexString(deviceCodeBytes);

        return new DeviceAuthTransactionDto(
            userCode,
            deviceCode,
            DeviceInfo.Create(
                deviceDto.UserAgent,
                deviceDto.Device,
                deviceDto.OS,
                deviceDto.Browser
            ),
            deviceDto.Ip,
            null,
            false,
            false
        );
    }
}