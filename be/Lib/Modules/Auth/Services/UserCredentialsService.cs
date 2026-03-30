using Lib.Exceptions;
using Lib.Modules.Auth.DTOs;
using Lib.Modules.Auth.Entities;
using Lib.Modules.Auth.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Lib.Modules.Auth.Services;

public class UserCredentialsService(
    IUserCredentialsRepository repository,
    IPasswordHasher<UserCredentials> hasher
) : IUserCredentialsService
{
    public async Task CreateAsync(UserCredentialsCreateDto createDto)
    {
        var hashedPassword = hasher.HashPassword(null!, createDto.PlainPassword);
        
        var credentials = UserCredentials.Create(
            createDto.UserId,
            hashedPassword
        );
        
        await repository.AddAsync(credentials);
        await repository.SaveChangesAsync();
    }

    public async Task UpdatePasswordAsync(UserCredentialsUpdateDto updateDto)
    {
        var credentials = await repository.GetByUserIdAsync(updateDto.UserId);
        if (credentials is null) throw new NotFoundException($"Credentials not found for user {updateDto.UserId}");
        
        var hashedPassword = hasher.HashPassword(null!, updateDto.NewPassword);
        
        credentials.UpdatePassword(hashedPassword);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId)
    {
        var credentials = await repository.GetByUserIdAsync(userId);
        if (credentials is not null)
        {
            repository.Delete(credentials);
            await repository.SaveChangesAsync();
        }
    }

    public async Task<bool> VerifyAsync(UserCredentialsVerifyDto verifyDto)
    {
        var credentials = await repository.GetByUserIdAsync(verifyDto.UserId);
        if (credentials is null) return false;

        var result = hasher.VerifyHashedPassword(null!, credentials.PasswordHash, verifyDto.PlainPassword);
        
        if (result == PasswordVerificationResult.Failed) return false;
        
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            string upgradedHash = hasher.HashPassword(null!, verifyDto.PlainPassword);
            credentials.UpdatePassword(upgradedHash);
            await repository.SaveChangesAsync();
        }

        return true;
    }
}