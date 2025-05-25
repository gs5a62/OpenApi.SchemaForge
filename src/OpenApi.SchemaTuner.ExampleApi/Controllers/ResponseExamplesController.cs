using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenApi.SchemaTuner.Core.Abstractions;
using OpenApi.SchemaTuner.Core.Attributes;
using OpenApi.SchemaTuner.Core.Extensions;

namespace OpenApi.SchemaTuner.ExampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResponseExamplesController : ControllerBase
{
    [HttpGet("clients")]
    [ProducesResponseType<ClientDto>(200)]
    public IActionResult GetUserById()
    {
        return Ok();
    }
    
    [HttpPost("clients")]
    [OpenApiResponse("When the client already exists", HttpStatusCode.Conflict)]
    public IActionResult RegisterAccount()
    {
        return Ok();
    }
    
    [AllowAnonymous]
    [HttpPost("user/confirm-register")]
    [OpenApiRequestExample<CreateClientExamples>]
    public IActionResult ConfirmRegisterAccount()
    {
        return Ok();
    }
}

public class ClientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}

public class CreateClientExamples : IOpenApiRequestExamples
{
    public static IList<OpenApiRequestExample> Examples { get; set; } = new List<OpenApiRequestExample>
    {
        new()
        {
            Message = "creating main client",
            Example = new
            {
                ParentAccountId = Guid.NewGuid(),
                Name = "Main account 1",
                Description = "Main account 1 desc.",
                IsMainAccount = true
            }.ToJson(true)
        },
        new()
        {
            Message = "creating sub client",
            Example = new
            {
                ParentAccountId = Guid.NewGuid(),
                Name = "Sub account 1",
                Description = "Sub account 1 desc.",
                IsMainAccount = false,
                InitialBalanceCredit = 1500,
                InitialBalanceDebit = 0
            }.ToJson(true)
        }
    };
}