using System.Text.RegularExpressions;
using Addressing.Net.Abstractions;
using Addressing.Net.Model;

namespace Addressing.Net.Runtime;

/// <summary>
/// Provides metadata and validation logic for country-specific address formats.
/// </summary>
public sealed class AddressMetadataProvider : IAddressMetadataProvider
{
    private readonly CountryIndex _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressMetadataProvider"/> class using embedded JSON data.
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="AddressMetadataProvider"/> class with a custom dataset.
    /// </summary>
    /// <param name="data">An enumerable of key-value pairs containing country codes and their address metadata.</param>
    public AddressMetadataProvider(IEnumerable<KeyValuePair<string, CountryAddressMetadata>> data)
    {
        var dict = data.ToDictionary(
            kv => kv.Key.ToUpperInvariant(),
            kv => Normalize(kv.Key, kv.Value),
            StringComparer.OrdinalIgnoreCase);

        _index = new CountryIndex(dict);
    }

    /// <summary>
    /// Normalizes the country address metadata by ensuring the key is uppercase and not null or whitespace.
    /// </summary>
    /// <param name="cc">The country code.</param>
    /// <param name="meta">The country address metadata to normalize.</param>
    /// <returns>The normalized <see cref="CountryAddressMetadata"/>.</returns>
    private static CountryAddressMetadata Normalize(string cc, CountryAddressMetadata meta)
    {
        meta.Key = string.IsNullOrWhiteSpace(meta.Key) ? cc.ToUpperInvariant() : meta.Key!.ToUpperInvariant();
        return meta;
    }

    /// <summary>
    /// Gets the postal code regular expression for the specified country code.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <returns>The postal code <see cref="Regex"/> if available; otherwise, <c>null</c>.</returns>
    public Regex? GetPostalRegex(string countryCode)
        => _index.PostalRegex(countryCode);

    /// <summary>
    /// Determines whether the specified postal code is valid for the given country code.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <param name="postal">The postal code to validate.</param>
    /// <returns><c>true</c> if the postal code is valid; otherwise, <c>false</c>.</returns>
    public bool IsValidPostal(string countryCode, string? postal)
    {
        if (string.IsNullOrWhiteSpace(postal)) return false;
        var rx = GetPostalRegex(countryCode);
        // If we have a pattern, enforce it; otherwise be permissive.
        return rx is null ? postal.Trim().Length is >= 3 and <= 16 : rx.IsMatch(postal.Trim());
    }

    /// <summary>
    /// Gets the subdivision keys for the specified country code.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <returns>A read-only list of subdivision keys.</returns>
    public IReadOnlyList<string> GetSubdivisionKeys(string countryCode)
        => _index.Subdivisions(countryCode).keys;

    /// <summary>
    /// Gets the subdivision names for the specified country code.
    /// </summary>
    /// <param name="countryCode">The country code.</param>
    /// <returns>A read-only dictionary mapping subdivision keys to names.</returns>
    public IReadOnlyDictionary<string, string> GetSubdivisionNames(string countryCode)
        => _index.Subdivisions(countryCode).names;


    /// <summary>
    /// Determines whether a given subdivision code is valid for the specified country.
    /// </summary>
    /// <param name="countryCode">
    /// The ISO 3166-1 alpha-2 country code (e.g. "US", "NG").
    /// </param>
    /// <param name="subdivisionCode">
    /// The subdivision code to validate (e.g. state or province code). 
    /// Case-insensitive. May be <c>null</c> or whitespace.
    /// </param>
    /// <returns>
    /// <c>true</c> if the subdivision code exists for the given country; otherwise, <c>false</c>.
    /// </returns>
    public bool IsValidSubdivision(string countryCode, string? subdivisionCode)
    {
        if (string.IsNullOrWhiteSpace(subdivisionCode)) return false;
        var keys = GetSubdivisionKeys(countryCode);
        return keys.Contains(subdivisionCode.Trim(), StringComparer.OrdinalIgnoreCase);
    }
}
