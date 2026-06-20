using Lib.Modules.Releases.DTOs;
using Lib.Modules.Releases.Entities;
using Lib.Modules.Releases.Repositories;
using Lib.Shared.DTOs;

namespace Lib.Modules.Releases.Services;

public interface IReleaseService
{
    public Task<PagedResultDto<ReleaseDto>> GetAllAsync(ReleaseFilter filter);
    public Task<ReleaseDto?> GetLatestAsync(Guid projectId);
    public Task<Release> GetRawByIdAsync(Guid id);
    public Task<ReleaseFile> GetRawFileByIdAsync(Guid id);
    public Task<ReleaseDto> CreateAsync(Guid projectId, Guid authorId, CreateReleaseDto dto);
    public Task DeleteAsync(Guid id);
}