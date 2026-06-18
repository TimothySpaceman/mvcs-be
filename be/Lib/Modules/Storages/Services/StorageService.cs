using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Entities;
using Lib.Modules.Storages.Repositories;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Storages.Services;

public class StorageService(IStorageRepository repository) : IStorageService
{
    public async Task<List<StorageDto>> GetAllByUserIdAsync(Guid userId)
    {
        var storages = await repository.GetAllByUserIdAsync(userId);
        return storages.Select(s => StorageDto.FromStorage(s, userId)).ToList();
    }

    public async Task<StorageDto?> GetByIdAsync(Guid id, Guid? userId = null)
    {
        var storage = await repository.GetByIdAsync(id);
        return storage is null ? null : StorageDto.FromStorage(storage, userId);
    }

    public async Task<Storage> GetRawByIdAsync(Guid id)
    {
        var storage = await repository.GetByIdAsync(id);
        if (storage is null) throw new NotFoundException("Storage not found");
        return storage;
    }

    public async Task<StorageConfigDto> GetConfigAsync(Guid id)
    {
        var storage = await GetRawByIdAsync(id);
        return StorageConfigDto.FromStorage(storage);
    }

    public async Task<StorageDto> CreateAsync(Guid ownerId, StorageCreateDto createDto)
    {
        var storage = Storage.Create(createDto.Name, createDto.StorageTypeId, createDto.Config);
        var ownerAccess = StorageAccess.Create(storage.Id, ownerId, StorageAccessType.Owner);

        await repository.AddAsync(storage);
        await repository.AddAccessAsync(ownerAccess);
        await repository.SaveChangesAsync();

        var created = await repository.GetByIdAsync(storage.Id);
        if (created is null) throw new InvalidOperationException("Failed to create storage");
        return StorageDto.FromStorage(created, ownerId);
    }

    public async Task<StorageDto> UpdateAsync(
        Guid id,
        StorageUpdateDto updateDto,
        Guid? userId = null
    )
    {
        var storage = await GetRawByIdAsync(id);
        storage.Rename(updateDto.Name);
        return StorageDto.FromStorage(storage, userId);
    }

    public async Task UpdateConfigAsync(Guid id, StorageUpdateConfigDto updateDto)
    {
        var storage = await GetRawByIdAsync(id);
        storage.UpdateConfig(updateDto.Config);
        await repository.SaveChangesAsync();
    }

    public async Task GrantAccessAsync(Guid id, StorageGrantAccessDto grantDto)
    {
        var storage = await GetRawByIdAsync(id);

        if (grantDto.AccessType == StorageAccessType.Owner)
        {
            throw new BadRequestException("Cannot grant Owner access");
        }

        var existing = storage.AccessEntries.FirstOrDefault(a => a.UserId == grantDto.UserId);
        if (existing is not null)
        {
            existing.ChangeAccessType(grantDto.AccessType);
        }
        else
        {
            var newAccess = StorageAccess.Create(storage.Id, grantDto.UserId, grantDto.AccessType);
            await repository.AddAccessAsync(newAccess);
        }

        await repository.SaveChangesAsync();
    }

    public async Task RevokeAccessAsync(Guid id, Guid targetUserId)
    {
        var storage = await GetRawByIdAsync(id);

        var access = storage.AccessEntries.FirstOrDefault(a => a.UserId == targetUserId);
        if (access is null)
        {
            throw new NotFoundException("Access entry not found");
        }

        repository.DeleteAccess(access);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var storage = await GetRawByIdAsync(id);
        repository.Delete(storage);
        await repository.SaveChangesAsync();
    }
}