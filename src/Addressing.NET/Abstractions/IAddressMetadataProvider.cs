using System.Text.RegularExpressions;

namespace Addressing.Net.Abstractions;

public interface IAddressMetadataProvider
{
    Regex? GetPostalRegex(string countryCode);
    bool IsValidPostal(string countryCode, string? postal);
    IReadOnlyList<string> GetSubdivisionKeys(string countryCode);
    IReadOnlyDictionary<string, string> GetSubdivisionNames(string countryCode);
    bool IsValidSubdivision(string countryCode, string? subdivisionCode);
}
