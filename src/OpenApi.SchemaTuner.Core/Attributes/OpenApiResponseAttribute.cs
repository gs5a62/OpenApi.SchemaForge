using System.Net;

namespace OpenApi.SchemaTuner.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OpenApiResponseAttribute(string message, HttpStatusCode statusCode) : Attribute
{
    public string Message { get; set; } = message;
    public HttpStatusCode StatusCode { get; set; } = statusCode;
}