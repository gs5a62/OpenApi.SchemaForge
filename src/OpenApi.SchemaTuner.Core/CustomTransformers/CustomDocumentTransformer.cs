using System.Text;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace OpenApi.SchemaTuner.Core.CustomTransformers;

internal sealed class CustomDocumentTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        //todo: handle multiple docs and load from file or type

        var newPaths = new OpenApiPaths();

        foreach (var entry in document.Paths)
        {
            var lowerPath = ConvertPathToLowerExceptParameters(entry.Key);
            newPaths.Add(lowerPath, entry.Value);
        }

        document.Paths = newPaths;

        document.Tags = document.Tags.DistinctBy(t => t.Name).ToList();

        return Task.CompletedTask;
    }

    private static void NormalizeControllerNames(IList<OpenApiTag> tags)
    {
        foreach (var tag in tags)
        {
            tag.Name = tag.Name.ToLower();
        }
    }

    private static string ConvertPathToLowerExceptParameters(string route)
    {
        var segments = route.Split('/');
        var result = new StringBuilder();

        foreach (var segment in segments)
        {
            if (string.IsNullOrEmpty(segment))
            {
                result.Append('/');
                continue;
            }

            if (segment.StartsWith("{") && segment.EndsWith("}"))
            {
                result.Append(segment);
            }
            else
            {
                result.Append(segment.ToLowerInvariant());
            }

            result.Append('/');
        }

        if (!route.EndsWith("/"))
        {
            result.Length--;
        }

        return result.ToString();
    }
}