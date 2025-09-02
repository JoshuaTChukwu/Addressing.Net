using System.Text.RegularExpressions;
using Addressing.Net.Abstractions;

namespace Addressing.Net.Runtime;

/// Minimal no-data provider: validates only presence/length; accepts all subdivisions.
/// This gives you a working baseline that compiles and passes basic tests.
/// You can swap it out later for an embedded or remote (Chromium) provider.
public sealed class AddressMetadataProvider : IAddressMetadataProvider
{
    public Regex? GetPostalRegex(string countryCode) => null;

    public bool IsValidPostal(string countryCode, string? postal)
        => !string.IsNullOrWhiteSpace(postal) && postal.Trim().Length is >= 3 and <= 16;

    public IReadOnlyList<string> GetSubdivisionKeys(string countryCode)
        => Array.Empty<string>();

    public IReadOnlyDictionary<string, string> GetSubdivisionNames(string countryCode)
        => new Dictionary<string, string>();

    public bool IsValidSubdivision(string countryCode, string? subdivisionCode)
        => !string.IsNullOrWhiteSpace(subdivisionCode);
}
