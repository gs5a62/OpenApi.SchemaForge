using Microsoft.AspNetCore.Mvc;
using OpenApi.SchemaTuner.Core.Attributes;

namespace OpenApi.SchemaTuner.ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeprecationExamplesController : ControllerBase
{
    [HttpGet("users")]
    [OpenApiDeprecated(typeof(GroupingExamplesController), "GetUserById", "A deprecation optional message")]
    public IActionResult GetUsers()
    {
        return Ok();
    }
    
    [HttpGet("users-2")]
    public IActionResult GetUsersWithDeprecatedParam([FromQuery] SampleRequestWithDeprecatedParam query)
    {
        return Ok();
    }
}

public class SampleRequestWithDeprecatedParam
{
    [OpenApiDeprecated(message: "Use only username for filter")] public string? UserId { get; set; }
    /// <summary>
    /// Test Doc
    /// </summary>
    public string? Username { get; set; }
}