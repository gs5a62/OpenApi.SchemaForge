using System.Reflection;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using OpenApi.SchemaTuner.Core.Attributes;

namespace OpenApi.SchemaTuner.Core.CustomTransformers;

internal sealed class CustomSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        RemoveDefaultDescription(schema, context);
        HandleSchemaEnums(schema, context);
        HandleDataTypes(schema, context);
        HandleComplexResponseBodySchema(schema, context);

        return Task.CompletedTask;
    }

    private static void HandleDataTypes(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        if (context.JsonTypeInfo.Type == typeof(DateOnly))
        {
            schema.Type = "string";
            schema.Format = "date";
            schema.Example = null;
            schema.Description = "UTC date value";
        }

        if (context.JsonTypeInfo.Type == typeof(DateTime))
        {
            schema.Type = "string";
            schema.Format = "date-time";
            schema.Example = null;
            schema.Description = "UTC datetime value";
        }

        if (context.JsonTypeInfo.Type == typeof(TimeSpan))
        {
            schema.Type = "string";
            schema.Example = new OpenApiString("00:00:00");
            schema.Description = null;
        }

        if (context.JsonTypeInfo.Type == typeof(Guid))
        {
            schema.Type = "string";
            schema.Format = "uuid";
            schema.Example = null;
        }
    }

    private static void HandleSchemaEnums(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        var isEnum = context.JsonTypeInfo.Type.IsEnum;
        var isArrayOfEnum = context.JsonTypeInfo.Type.GetElementType()?.IsEnum is true;
        var isListOfEnum = context.JsonTypeInfo.Type.GenericTypeArguments.Length > 0 &&
                           context.JsonTypeInfo.Type.GenericTypeArguments[0].IsEnum;

        var modelType = context.ParameterDescription?
            .ModelMetadata?.ModelType;
        var nestedType = modelType?.GenericTypeArguments
            ?.FirstOrDefault();

        var isNestedEnum = nestedType?.IsEnum is true;

        if (isEnum || isListOfEnum || isArrayOfEnum || isNestedEnum)
        {
            var type = context.JsonTypeInfo.Type;
            if (isArrayOfEnum)
                type = context.JsonTypeInfo.Type.GetElementType();
            if (isListOfEnum)
                type = context.JsonTypeInfo.Type.GenericTypeArguments[0];
            if (isNestedEnum)
                type = nestedType;
            
            var isNullable = modelType is not null && Nullable.GetUnderlyingType(modelType) is not null;
            var enumDesc = string.Join(", ",
                Enum.GetValues(type).Cast<object>()
                    .Select(v => $"{Enum.GetName(type, v)} : {(int)v}"));

            var extraTypeString = string.Empty;
            if (isArrayOfEnum)
                extraTypeString = "array of";
            else if (isListOfEnum)
                extraTypeString = "list of";
            
            schema.Description = $"{type.Name} enum " + enumDesc;
            schema.Type = $"{extraTypeString} {type.Name}";
            schema.Format = $"enum{(isNullable ? " | null" : "")}";
        }
    }

    private static void HandleComplexResponseBodySchema(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        var type = context.JsonTypeInfo.Type;
        if (type.IsClass is false || type == typeof(string))
            return;

        foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var attr = property.GetCustomAttribute<OpenApiDescriptionAttribute>();
            if (attr == null) continue;

            var itemSchema = schema.Properties
                .First(p => p.Key.ToLower() == property.Name.ToLower());

            itemSchema.Value.Description = attr.Message;

            var propertyName = property.Name;

            if (schema.Properties.TryGetValue(propertyName, out var propSchema))
            {
                propSchema.Description = attr.Message;
            }
        }
    }

    private static void RemoveDefaultDescription(OpenApiSchema schema, OpenApiSchemaTransformerContext context)
    {
        if (context.ParameterDescription?.ModelMetadata?.Properties.Count is null or 0)
            schema.Description = " ";
    }
}