using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.Services;

public interface IStorageService
{
    public Task<List<StorageDto>> GetAllByUserIdAsync(Guid userId);
    public Task<StorageDto?> GetByIdAsync(Guid id, Guid userId);
    public Task<Storage> GetRawByIdAsync(Guid id, Guid userId);
    public Task<StorageConfigDto> GetConfigAsync(Guid id, Guid userId);
    public Task<StorageDto> CreateAsync(Guid ownerId, StorageCreateDto createDto);
    public Task<StorageDto> UpdateAsync(Guid id, Guid userId, StorageUpdateDto updateDto);
    public Task UpdateConfigAsync(Guid id, Guid userId, StorageUpdateConfigDto updateDto);
    public Task GrantAccessAsync(Guid id, Guid userId, StorageGrantAccessDto grantDto);
    public Task RevokeAccessAsync(Guid id, Guid userId, Guid targetUserId);
    public Task DeleteAsync(Guid id, Guid userId);
}