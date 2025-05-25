using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenApi.SchemaTuner.Core.Extensions;

public static class ClassExtensions
{
    public static string ToJson<T>(this T item, bool writeIndented = false)
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = writeIndented
        };

        return JsonSerializer.Serialize(item, options);
    }
}