using Lib.Modules.Storages.DTOs;
using Lib.Modules.Storages.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lib.Modules.Storages.Controllers;

[Authorize]
[ApiController]
[Route("api/storage-types")]
public class StorageTypesController(IStorageTypeService storageTypeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<StorageTypeInfoDto>>> GetAll()
    {
        var result = await storageTypeService.GetAllAsync();
        return Ok(result);
    }
 
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<StorageTypeDto>> GetById(Guid id)
    {
        var result = await storageTypeService.GetByIdAsync(id);
        if (result is null) return NotFound();
        return Ok(result);
    }
}
