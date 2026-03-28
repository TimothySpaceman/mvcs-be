using Microsoft.AspNetCore.Mvc;

namespace Api.Dummy;

[ApiController]
[Route("[controller]")]
public class DummyController : ControllerBase
{
    private readonly IConfiguration _config;

    public DummyController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    public IActionResult GetHello()
    {
        return Ok("Hello World!");
    }
    
    [HttpGet("connections")]
    public IActionResult GetConnectionStrings()
    {
        var section = _config.GetSection("ConnectionStrings");

        var result = section.GetChildren()
            .ToDictionary(x => x.Key, x => x.Value);

        return Ok(result);
    }
}