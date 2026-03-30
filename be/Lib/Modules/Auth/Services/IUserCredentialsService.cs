using Lib.Modules.Auth.DTOs;

namespace Lib.Modules.Auth.Services;

public interface IUserCredentialsService
{
    public Task CreateAsync (UserCredentialsCreateDto createDto);
    public Task UpdatePasswordAsync(UserCredentialsUpdateDto updateDto);
    public Task DeleteAsync(Guid userId);
    public Task<bool> VerifyAsync(UserCredentialsVerifyDto verifyDto);
}