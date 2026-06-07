using Lib.Modules.Users.DTOs;
using Lib.Shared.DTOs;

namespace Lib.Modules.Users.Services;

public interface IUserService
{
    public Task<PagedResultDto<UserDto>> GetAllAsync(int page, int itemsPerPage);
    public Task<List<UserDto>> GetAllByIdsAsync(IEnumerable<Guid> ids);
    public Task<UserDto?> GetByIdAsync(Guid id);
    public Task<UserDto?> GetByEmailAsync(string email);
    public Task<UserDto?> GetByUsernameAsync(string username);
    public Task<bool> ExistsByEmailAsync(string email);
    public Task<bool> ExistsByUsernameAsync(string username);
    public Task<UserDto> CreateAsync(UserCreateDto createDto);
    public Task<UserDto> UpdateByIdAsync(Guid id, UserUpdateDto updateDto);
    public Task DeleteByIdAsync(Guid id, bool soft = false);
}