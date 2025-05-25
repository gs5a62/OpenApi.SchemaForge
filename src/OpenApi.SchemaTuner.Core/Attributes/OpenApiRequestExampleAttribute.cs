namespace OpenApi.SchemaTuner.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class OpenApiRequestExampleAttribute<T> : Attribute
{
    public Type ExampleType { get; set; } = typeof(T);
}