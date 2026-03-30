using Lib.Modules.Users.DTOs;
using Lib.Modules.Users.Entities;
using Lib.Modules.Users.Repositories;

namespace Lib.Modules.Users.Services;

public class UserService(IUserRepository repository) : IUserService
{
    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await repository.GetByIdAsync(id);
        return user is null ? null : UserDto.FromUser(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await repository.GetByEmailAsync(email);
        return user is null ? null : UserDto.FromUser(user);
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var user = await repository.GetByUsernameAsync(username);
        return user is null ? null : UserDto.FromUser(user);
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        return repository.ExistsByEmailAsync(email);
    }

    public Task<bool> ExistsByUsernameAsync(string username)
    {
        return repository.ExistsByUsernameAsync(username);
    }

    public async Task<UserDto> CreateAsync(UserCreateDto createDto)
    {
        var user = User.Create(
            createDto.Username,
            createDto.DisplayName,
            createDto.Email
        );
        await repository.AddAsync(user);
        await repository.SaveChangesAsync();
        return UserDto.FromUser(user);
    }

    public async Task<UserDto> UpdateByIdAsync(Guid id, UserUpdateDto updateDto)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) throw new KeyNotFoundException("User not found");
        user.UpdateProfile(updateDto.DisplayName);
        await repository.SaveChangesAsync();
        return UserDto.FromUser(user);
    }

    public async Task DeleteByIdAsync(Guid id, bool soft = true)
    {
        var user = await repository.GetByIdAsync(id);
        if (user is null) throw new KeyNotFoundException("User not found");
        
        if (soft)
        {
            user.Delete();    
        }
        else
        {
            repository.Delete(user);
        }
        
        await repository.SaveChangesAsync();
    }
}