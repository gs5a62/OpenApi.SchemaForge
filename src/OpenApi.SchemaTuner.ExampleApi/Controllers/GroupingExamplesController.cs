using Microsoft.AspNetCore.Mvc;
using OpenApi.SchemaTuner.Core.Attributes;

namespace OpenApi.SchemaTuner.ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupingExamplesController : ControllerBase
{
    [HttpGet("users/{userId}")]
    public IActionResult GetUserById()
    {
        return Ok();
    }
    
    [HttpPost("user/register")]
    [OpenApiGroup("Registration Flow")]
    public IActionResult RegisterAccount()
    {
        return Ok();
    }
    
        
    [HttpPost("user/confirm-register")]
    [OpenApiGroup("Registration Flow")]
    public IActionResult ConfirmRegisterAccount()
    {
        return Ok();
    }
}