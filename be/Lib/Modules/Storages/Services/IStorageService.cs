using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Entities;

namespace Lib.Modules.Storages.Services;

public interface IStorageService
{
    public Task<List<StorageDto>> GetAllByUserIdAsync(Guid userId);
    public Task<StorageDto?> GetByIdAsync(Guid id);
    public Task<Storage> GetRawByIdAsync(Guid id);
    public Task<StorageConfigDto> GetConfigAsync(Guid id);
    public Task<StorageDto> CreateAsync(Guid ownerId, StorageCreateDto createDto);
    public Task<StorageDto> UpdateAsync(Guid id, StorageUpdateDto updateDto);
    public Task UpdateConfigAsync(Guid id, StorageUpdateConfigDto updateDto);
    public Task GrantAccessAsync(Guid id, StorageGrantAccessDto grantDto);
    public Task RevokeAccessAsync(Guid id, Guid targetUserId);
    public Task DeleteAsync(Guid id);
}