namespace OpenApi.SchemaTuner.Core.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class OpenApiDescriptionAttribute(string message) : Attribute
{
    public string Message { get; set; } = message;
}