using Lib.Modules.Users.DTOs;

namespace Lib.Modules.Users.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<bool> ExistsByUsernameAsync(string username);
    Task CreateAsync(UserCreateDto createDto);
    Task UpdateByIdAsync(Guid id, UserUpdateDto updateDto);
    Task DeleteByIdAsync(Guid id, bool soft = false);
}