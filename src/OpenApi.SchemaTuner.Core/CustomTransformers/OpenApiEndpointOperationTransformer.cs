using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using OpenApi.SchemaTuner.Core.Abstractions;
using OpenApi.SchemaTuner.Core.Attributes;

namespace OpenApi.SchemaTuner.Core.CustomTransformers;

internal class OpenApiEndpointOperationTransformer : IOpenApiOperationTransformer
{
    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context,
        CancellationToken cancellationToken)
    {
        operation.Description = "<div>";

        NormalizeControllerNames(operation);
        HandleEndpointGroups(operation, context);
        HandleAnonymousEndpoints(operation, context);
        HandleEndpointResponses(operation, context);

        HandleEndpointDescription(operation, context);
        HandleEndpointDeprecations(operation, context);
        HandleEndpointExamples(operation, context);

        operation.Description += "</div>";

        return Task.CompletedTask;
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

    private static void NormalizeControllerNames(OpenApiOperation operation)
    {
        foreach (var tag in operation.Tags)
        {
            tag.Name = tag.Name.ToLower();
        }
    }

    private static void HandleEndpointGroups(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        var groupAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<OpenApiGroupAttribute>()
            .FirstOrDefault();

        if (groupAttribute is not null)
        {
            operation.Tags.Add(new OpenApiTag
            {
                Name = "[#] " + groupAttribute.TagName
            });
        }
    }

    private static void HandleAnonymousEndpoints(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        var anonymousAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<AllowAnonymousAttribute>()
            .FirstOrDefault();

        if (anonymousAttribute is not null)
        {
            operation.Description += "<pre><sup>🟢 GUEST</sup></pre>";
        }
    }

    private static void HandleEndpointResponses(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        var openApiResponseAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<OpenApiResponseAttribute>()
            .FirstOrDefault();

        if (openApiResponseAttribute is not null)
        {
            operation.Responses.Add(openApiResponseAttribute.StatusCode.ToString(),
                new OpenApiResponse { Description = openApiResponseAttribute.Message });
        }
    }

    private static void HandleEndpointDescription(OpenApiOperation operation,
        OpenApiOperationTransformerContext context)
    {
        var openApiDescriptionAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<OpenApiDescriptionAttribute>()
            .FirstOrDefault();

        if (openApiDescriptionAttribute is not null)
            operation.Description += $"<pre>{openApiDescriptionAttribute.Message}</pre>";
    }

    private static void HandleEndpointDeprecations(OpenApiOperation operation,
        OpenApiOperationTransformerContext context)
    {
        var obsoleteAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .OfType<OpenApiDeprecatedAttribute>()
            .FirstOrDefault();

        if (obsoleteAttribute is not null)
        {
            operation.Deprecated = true;
            operation.Tags.Add(new OpenApiTag
            {
                Name = "[x] deprecated",
            });

            var obsoleteMessage = $"ⓘ {obsoleteAttribute.Message}";
            var controllerName = obsoleteAttribute.NewController?.Name.Replace("Controller", "").ToLower();
            var methodInfo = obsoleteAttribute.NewController?.GetMethod(obsoleteAttribute.NewMethodName);
            var httpMethod = GetMethodName(methodInfo);

            var url = $"#tag/{controllerName}/{httpMethod.httpMethod}/api/{controllerName}/{httpMethod.route}";
            var obsoleteUrl =
                $" see <a href='{url.TrimEnd('/')}'>[{httpMethod.httpMethod?.ToLowerInvariant()}] api/{controllerName}/{httpMethod.route}</a>";
            //todo: get controller full actual path instead of assume it start with [API]
            operation.Description += $"<div>{obsoleteMessage} {obsoleteUrl}</div>";
        }
    }

    private static void HandleEndpointExamples(OpenApiOperation operation, OpenApiOperationTransformerContext context)
    {
        var openApiExampleRequestAttribute = context.Description.ActionDescriptor.EndpointMetadata
            .FirstOrDefault(meta =>
                meta.GetType().IsGenericType &&
                meta.GetType().GetGenericTypeDefinition() == typeof(OpenApiRequestExampleAttribute<>));

        if (openApiExampleRequestAttribute is not null)
        {
            var prop = openApiExampleRequestAttribute.GetType().GenericTypeArguments.First().GetProperty(
                nameof(IOpenApiRequestExamples.Examples), BindingFlags.Public | BindingFlags.Static);

            var examples = (IList<OpenApiRequestExample>)prop?.GetValue(null);

            var examplesMarkdown = "";
            foreach (var example in examples ?? [])
            {
                if (string.IsNullOrWhiteSpace(example.Message) is false)
                    examplesMarkdown +=
                        $"<pre>ex : {example.Message}</pre>";

                if (string.IsNullOrWhiteSpace(example.Example) is false)
                    examplesMarkdown +=
                        $"<pre><code>{example.Example}</code></pre>";
            }

            operation.Description = $"<details><summary>Examples</summary>{examplesMarkdown}</details>";
        }
    }
}