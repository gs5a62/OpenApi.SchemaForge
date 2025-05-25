namespace OpenApi.SchemaTuner.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class OpenApiGroupAttribute(string tagName) : Attribute
{
    public string TagName { get; set; } = tagName;
}