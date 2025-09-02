using System.Reflection;
using System.Text.Json;

namespace Addressing.Net.Runtime;

internal static class EmbeddedResourceLoader
{
    public static IEnumerable<(string Code, T Data)> LoadAll<T>(Assembly asm, string folder)
    {
        var prefix = $"{asm.GetName().Name}.{folder.Replace('\\', '.')}.";
        foreach (var name in asm.GetManifestResourceNames())
        {
            if (!name.StartsWith(prefix, StringComparison.Ordinal)) continue;

            using var stream = asm.GetManifestResourceStream(name)!;
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var data = JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            if (data is null) continue;

            // Extract "US" from "Addressing.NET.data.US.json"
            var codeWithExt = name.Substring(prefix.Length); // e.g. "US.json"
            var code = codeWithExt.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? codeWithExt.Substring(0, codeWithExt.Length - 5)
                : codeWithExt;
            code = code.ToUpperInvariant();

            yield return (code, data);
        }
    }
}
