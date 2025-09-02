using System.Text.RegularExpressions;
using Addressing.Net.Abstractions;
using Addressing.Net.Model;

namespace Addressing.Net.Runtime;

public sealed class AddressMetadataProvider : IAddressMetadataProvider
{
    private readonly CountryIndex _index;

    // Loads JSON embedded under src/Addressing.NET/data/*.json
    public AddressMetadataProvider()
    {
        var asm = typeof(AddressMetadataProvider).Assembly;

        var dict = EmbeddedResourceLoader
            .LoadAll<CountryAddressMetadata>(asm, "data")
            .ToDictionary(
                x => x.Code.ToUpperInvariant(),
                x => Normalize(x.Code, x.Data),
                StringComparer.OrdinalIgnoreCase);

        _index = new CountryIndex(dict);
    }

    // Overload for tests / supplying your own dataset
    public AddressMetadataProvider(IEnumerable<KeyValuePair<string, CountryAddressMetadata>> data)
    {
        var dict = data.ToDictionary(
            kv => kv.Key.ToUpperInvariant(),
            kv => Normalize(kv.Key, kv.Value),
            StringComparer.OrdinalIgnoreCase);

        _index = new CountryIndex(dict);
    }

    private static CountryAddressMetadata Normalize(string cc, CountryAddressMetadata meta)
    {
        meta.Key = string.IsNullOrWhiteSpace(meta.Key) ? cc.ToUpperInvariant() : meta.Key!.ToUpperInvariant();
        return meta;
    }

    public Regex? GetPostalRegex(string countryCode)
        => _index.PostalRegex(countryCode);

    public bool IsValidPostal(string countryCode, string? postal)
    {
        if (string.IsNullOrWhiteSpace(postal)) return false;
        var rx = GetPostalRegex(countryCode);
        // If we have a pattern, enforce it; otherwise be permissive.
        return rx is null ? postal.Trim().Length is >= 3 and <= 16 : rx.IsMatch(postal.Trim());
    }

    public IReadOnlyList<string> GetSubdivisionKeys(string countryCode)
        => _index.Subdivisions(countryCode).keys;

    public IReadOnlyDictionary<string, string> GetSubdivisionNames(string countryCode)
        => _index.Subdivisions(countryCode).names;

    public bool IsValidSubdivision(string countryCode, string? subdivisionCode)
    {
        if (string.IsNullOrWhiteSpace(subdivisionCode)) return false;
        var keys = GetSubdivisionKeys(countryCode);
        return keys.Contains(subdivisionCode.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
