using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using OpenApi.SchemaTuner.Core.Attributes;

namespace OpenApi.SchemaTuner.Core.CustomTransformers;

internal class OpenApiEndpointDetailsOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        foreach (var parameter in context.Description.ParameterDescriptions)
        {
            HandleDeprecatedRequestParameters(operation, parameter);
            HandleIgnoredRequestParameters(operation, parameter);
            DescribeRequestEnums(operation);
            HandleRequestParameterDescription(operation, parameter);
        }

        return Task.CompletedTask;
    }

    private static void HandleDeprecatedRequestParameters(OpenApiOperation operation, ApiParameterDescription parameter)
    {
        var att = (parameter.ModelMetadata as DefaultModelMetadata)?.Attributes
            .Attributes
            .OfType<OpenApiDeprecatedAttribute>()
            .FirstOrDefault();

        if (att is null) return;

        var pp = operation.Parameters.First(p => p.Name == parameter.Name);
        pp.Deprecated = true;
        pp.Description += $"<div>ⓘ {att.Message}</div>";
    }

    private static void HandleIgnoredRequestParameters(OpenApiOperation operation, ApiParameterDescription parameter)
    {
        var att = (parameter.ModelMetadata as DefaultModelMetadata)?.Attributes
            .Attributes
            .OfType<OpenApiIgnoreAttribute>()
            .FirstOrDefault();

        if (att is null) return;

        var pp = operation.Parameters.First(p => p.Name == parameter.Name);
        operation.Parameters.Remove(pp);
    }

    private static void HandleRequestParameterDescription(OpenApiOperation operation, ApiParameterDescription parameter)
    {
        var pp = operation.Parameters?.FirstOrDefault(p => p.Name == parameter.Name);

        if (pp is null)
            return;

        var att = (parameter.ModelMetadata as DefaultModelMetadata)?.Attributes
            .Attributes
            .OfType<OpenApiDescriptionAttribute>()
            .FirstOrDefault();

        if (att is null)
        {
            pp.Description ??= " ";
            return;
        }

        pp.Description += $"<pre>{att.Message}</pre>";
    }

    private static void DescribeRequestEnums(OpenApiOperation operation)
    {
        foreach (var pp in operation.Parameters ?? [])
        {
            if (pp.Schema.Format?.StartsWith("enum") is not true)
                continue;

            var enumTypeName = pp.Schema.Items is null ? pp.Schema.Type : pp.Schema.Items.Type;

            var enumType = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == enumTypeName.Trim() && t.IsEnum);

            if (enumType is null)
                continue;

            var enumDesc = string.Join("",
                Enum.GetValues(enumType).Cast<object>()
                    .Select(v => $"<li><code>{(int)v} {Enum.GetName(enumType, v)}</code></li>"));

            pp.Description = $"<ul>{enumDesc} </ul>";
        }
    }

    private static (string? httpMethod, string? route) GetMethodName(MethodInfo? methodInfo)
    {
        var attribute = methodInfo?
            .CustomAttributes
            .FirstOrDefault(a => typeof(HttpMethodAttribute).IsAssignableFrom(a.AttributeType));
        var httpMethod = attribute?.AttributeType.Name
            .Replace("Http", "").Replace("Attribute", "").ToUpperInvariant();

        var route = attribute?.ConstructorArguments.FirstOrDefault().Value;
        return (httpMethod, route?.ToString());
    }
}