using Lib.Modules.Releases.DTOs;
using Lib.Modules.Releases.Entities;
using Lib.Modules.Releases.Repositories;
using Lib.Shared.DTOs;
using Lib.Shared.Exceptions;

namespace Lib.Modules.Releases.Services;

public class ReleaseService(IReleaseRepository repository) : IReleaseService
{
    public async Task<PagedResultDto<ReleaseDto>> GetAllAsync(ReleaseFilter filter)
    {
        var releases = await repository.GetAllAsync(filter);
        var total = await repository.CountAsync(filter);
        return new PagedResultDto<ReleaseDto>(
            releases.Select(ReleaseDto.FromEntity),
            filter.Page,
            filter.ItemsPerPage,
            total
        );
    }

    public async Task<ReleaseDto?> GetLatestAsync(Guid projectId)
    {
        var release = await repository.GetLatestByProjectIdAsync(projectId);
        return release is null ? null : ReleaseDto.FromEntity(release);
    }

    public async Task<Release> GetRawByIdAsync(Guid id)
    {
        var release = await repository.GetByIdAsync(id);
        if (release is null) throw new NotFoundException("Release not found");
        return release;
    }

    public async Task<ReleaseFile> GetRawFileByIdAsync(Guid id)
    {
        var file = await repository.GetFileByIdAsync(id);
        if (file is null) throw new NotFoundException("Release file not found");
        return file;
    }

    public async Task<ReleaseDto> CreateAsync(Guid projectId, Guid authorId, CreateReleaseDto dto)
    {
        var release = Release.Create(projectId, authorId, dto.Title);

        foreach (var file in dto.Files)
        {
            release.AddFile(file.FileName, file.BlobId);
        }

        await repository.AddAsync(release);
        await repository.SaveChangesAsync();
        return ReleaseDto.FromEntity(release);
    }

    public async Task DeleteAsync(Guid id)
    {
        var release = await GetRawByIdAsync(id);
        repository.Delete(release);
        await repository.SaveChangesAsync();
    }
}