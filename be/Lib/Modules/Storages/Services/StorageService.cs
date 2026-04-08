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

    public async Task<StorageDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var storage = await repository.GetByIdWithAccessAsync(id, userId);
        return storage is null ? null : StorageDto.FromStorage(storage, userId);
    }

    public async Task<StorageConfigDto> GetConfigAsync(Guid id, Guid userId)
    {
        var storage = await GetStorageWithWriteAccessOrThrowAsync(id, userId);
        return StorageConfigDto.FromStorage(storage);
    }

    public async Task<StorageDto> CreateAsync(Guid ownerId, StorageCreateDto createDto)
    {
        var storage = Storage.Create(createDto.Name, createDto.StorageTypeId, createDto.Config);
        var ownerAccess = StorageAccess.Create(storage.Id, ownerId, StorageAccessType.Owner);

        await repository.AddAsync(storage);
        await repository.AddAccessAsync(ownerAccess);
        await repository.SaveChangesAsync();

        var created = await repository.GetByIdWithAccessAsync(storage.Id, ownerId);
        if (created is null)
        {
            throw new InvalidOperationException("Failed to create storage");
        }

        return StorageDto.FromStorage(created, ownerId);
    }

    public async Task<StorageDto> UpdateAsync(Guid id, Guid userId, StorageUpdateDto updateDto)
    {
        var storage = await GetStorageWithWriteAccessOrThrowAsync(id, userId);
        storage.Rename(updateDto.Name);
        await repository.SaveChangesAsync();
        return StorageDto.FromStorage(storage, userId);
    }

    public async Task UpdateConfigAsync(Guid id, Guid userId, StorageUpdateConfigDto updateDto)
    {
        var storage = await GetStorageWithOwnerAccessOrThrowAsync(id, userId);
        storage.UpdateConfig(updateDto.Config);
        await repository.SaveChangesAsync();
    }

    public async Task GrantAccessAsync(Guid id, Guid userId, StorageGrantAccessDto grantDto)
    {
        var storage = await GetStorageWithOwnerAccessOrThrowAsync(id, userId);

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

    public async Task RevokeAccessAsync(Guid id, Guid userId, Guid targetUserId)
    {
        var storage = await GetStorageWithOwnerAccessOrThrowAsync(id, userId);

        if (targetUserId == userId)
        {
            throw new BadRequestException("Cannot revoke your own owner access");
        }

        var access = storage.AccessEntries.FirstOrDefault(a => a.UserId == targetUserId);
        if (access is null)
        {
            throw new NotFoundException("Access entry not found");
        }

        repository.DeleteAccess(access);
        await repository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var storage = await GetStorageWithOwnerAccessOrThrowAsync(id, userId);
        repository.Delete(storage);
        await repository.SaveChangesAsync();
    }

    private async Task<Storage> GetStorageWithWriteAccessOrThrowAsync(Guid id, Guid userId)
    {
        var storage = await repository.GetByIdWithAccessAsync(id, userId);
        if (storage is null)
        {
            throw new NotFoundException("Storage not found or access denied");
        }

        if (storage.IsPublic) return storage;
        
        var access = storage.AccessEntries.First(a => a.UserId == userId);
        if (access.CanWrite)
        {
            return storage;
        }

        throw new ForbiddenException("Write access required");
    }

    private async Task<Storage> GetStorageWithOwnerAccessOrThrowAsync(Guid id, Guid userId)
    {
        var storage = await repository.GetByIdWithAccessAsync(id, userId);
        if (storage is null)
        {
            throw new NotFoundException("Storage not found or access denied");
        }
        
        if (storage.IsPublic)
        {
            throw new ForbiddenException("Public storages cannot be managed by users");
        }

        var access = storage.AccessEntries.First(a => a.UserId == userId);
        if (access.IsOwner)
        {
            return storage;
        }

        throw new ForbiddenException("Owner access required");
    }
}