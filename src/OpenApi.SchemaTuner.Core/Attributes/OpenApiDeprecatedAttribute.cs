namespace OpenApi.SchemaTuner.Core.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
public class OpenApiDeprecatedAttribute(
    Type? newController = null,
    string? newMethodName = null,
    string? message = null)
    : Attribute
{
    public Type? NewController { get; } = newController;
    public string? NewMethodName { get; } = newMethodName;
    public string? Message { get; } = message;
}