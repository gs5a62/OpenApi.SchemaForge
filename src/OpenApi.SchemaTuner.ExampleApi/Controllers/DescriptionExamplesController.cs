using Microsoft.AspNetCore.Mvc;
using OpenApi.SchemaTuner.Core.Attributes;

namespace OpenApi.SchemaTuner.ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DescriptionExamplesController : ControllerBase
{
    [OpenApiDescription("Fetch only enabled entities")]
    [HttpGet]
    public IActionResult EndpointWithDescription()
    {
        return Ok();
    }
    
    [HttpGet("list")]
    public IActionResult EndpointWithQueryParamDescription([FromQuery] RequestWithDescription request)
    {
        return Ok();
    }

    [HttpGet("list2")]
    public IActionResult EndpointWithBodyParamDescription([FromBody] OtherRequestWithDescription requestBody)
    {
        return Ok();
    }

    public class RequestWithDescription
    {
        [OpenApiDescription("The name of the client")]
        public string? Name { get; set; }
    }
    
    public class OtherRequestWithDescription
    {
        [OpenApiDescription("The expire date")]
        public DateTime? FromDate { get; set; }
    }
}