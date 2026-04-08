using Lib.Modules.Storages.DTOs;

namespace Lib.Modules.Storages.Services;

public interface IStorageService
{
    Task<List<StorageDto>> GetAllByUserIdAsync(Guid userId);
    Task<StorageDto?> GetByIdAsync(Guid id, Guid userId);
    Task<StorageConfigDto> GetConfigAsync(Guid id, Guid userId);
    Task<StorageDto> CreateAsync(Guid ownerId, StorageCreateDto createDto);
    Task<StorageDto> UpdateAsync(Guid id, Guid userId, StorageUpdateDto updateDto);
    Task UpdateConfigAsync(Guid id, Guid userId, StorageUpdateConfigDto updateDto);
    Task GrantAccessAsync(Guid id, Guid userId, StorageGrantAccessDto grantDto);
    Task RevokeAccessAsync(Guid id, Guid userId, Guid targetUserId);
    Task DeleteAsync(Guid id, Guid userId);
}