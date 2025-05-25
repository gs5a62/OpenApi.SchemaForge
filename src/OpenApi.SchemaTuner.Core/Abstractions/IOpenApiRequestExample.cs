using OpenApi.SchemaTuner.Core.Abstractions;

public interface IOpenApiRequestExamples
{
    static abstract IList<OpenApiRequestExample> Examples { get; set; }
}