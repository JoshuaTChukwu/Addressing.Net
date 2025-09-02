using System.Text.RegularExpressions;
using Addressing.Net.Model;

namespace Addressing.Net.Runtime;

internal sealed class CountryIndex
{
    private readonly Dictionary<string, CountryAddressMetadata> _map;

    public CountryIndex(Dictionary<string, CountryAddressMetadata> map)
        => _map = map;

    private CountryAddressMetadata? Get(string cc)
        => _map.TryGetValue(cc.ToUpperInvariant(), out var v) ? v : null;

    public Regex? PostalRegex(string cc)
    {
        var meta = Get(cc);
        var pattern = meta?.PostalCodePattern;
        if (string.IsNullOrWhiteSpace(pattern)) return null;

        return new Regex("^" + pattern + "$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    public (IReadOnlyList<string> keys, IReadOnlyDictionary<string, string> names) Subdivisions(string cc)
    {
        var meta = Get(cc);
        var keysRaw = meta?.SubKeys;
        if (string.IsNullOrWhiteSpace(keysRaw))
            return (Array.Empty<string>(), new Dictionary<string, string>());

        var namesRaw = meta!.SubNames ?? string.Empty;

        // netstandard2.0: must pass char[] for Split with StringSplitOptions
        var keys = keysRaw.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
        var names = namesRaw.Split(new[] { '~' }, StringSplitOptions.None);

        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < keys.Length; i++)
        {
            var display = (i < names.Length && !string.IsNullOrWhiteSpace(names[i])) ? names[i] : keys[i];
            dict[keys[i]] = display;
        }

        return (keys, dict);
    }
}
