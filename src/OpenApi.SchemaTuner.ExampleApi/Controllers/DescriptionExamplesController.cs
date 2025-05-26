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
    public IActionResult EndpointWithQueryParamDescription([FromQuery] int id, [FromQuery] RequestWithDescription request)
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
        public DateTime? DateBefore { get; set; }
        public DateTime? DateAfter { get; set; }
        public UserType? UserType1 { get; set; }
        public UserType UserType2 { get; set; }
        public IList<UserType> UserType3 { get; set; }
    }
    
    public class OtherRequestWithDescription
    {
        [OpenApiDescription("The expire date")]
        public DateTime? FromDate { get; set; }
    }
    
    public enum UserType
    {
        Admin,
        Guest
    }
}