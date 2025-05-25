using Microsoft.AspNetCore.OpenApi;
using OpenApi.SchemaTuner.Core.CustomTransformers;

namespace OpenApi.SchemaTuner.Core.Extensions;

public static class OpenApiOptionsExtensions
{
    /// <summary>
    /// Adds required Schema Tuner transformers to the <see cref="OpenApiOptions" />.
    /// </summary>
    /// <param name="options"><see cref="OpenApiOptions" />.</param>
    public static OpenApiOptions AddSchemaTunerTransformers(this OpenApiOptions options)
    {
        options.AddSchemaTransformer<CustomSchemaTransformer>();
        options.AddOperationTransformer<OpenApiEndpointDetailsOperationTransformer>();
        options.AddOperationTransformer<OpenApiEndpointOperationTransformer>();
        options.AddDocumentTransformer<CustomDocumentTransformer>();

        return options;
    }
}