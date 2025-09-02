using System.Text.RegularExpressions;

namespace Addressing.Net.Abstractions;

/// <summary>
/// Provides metadata and validation methods for address components such as postal codes and subdivisions.
/// </summary>
public interface IAddressMetadataProvider
{
    /// <summary>
    /// Gets the regular expression used to validate postal codes for the specified country.
    /// </summary>
    /// <param name="countryCode">The ISO country code.</param>
    /// <returns>A <see cref="Regex"/> for validating postal codes, or null if not available.</returns>
    Regex? GetPostalRegex(string countryCode);
    /// <summary>
    /// Validates the postal code for the specified country.
    /// </summary>
    /// <param name="countryCode">The ISO country code.</param>
    /// <param name="postal">The postal code to validate.</param>
    /// <returns>True if the postal code is valid for the country; otherwise, false.</returns>
    bool IsValidPostal(string countryCode, string? postal);
    /// <summary>
    /// Gets the list of subdivision keys for the specified country.
    /// </summary>
    /// <param name="countryCode">The ISO country code.</param>
    /// <returns>A read-only list of subdivision keys.</returns>
    IReadOnlyList<string> GetSubdivisionKeys(string countryCode);
    /// <summary>
    /// Gets a read-only dictionary of subdivision codes and their corresponding names for the specified country.
    /// </summary>
    /// <param name="countryCode">The ISO country code.</param>
    /// <returns>A read-only dictionary mapping subdivision codes to subdivision names.</returns>
    IReadOnlyDictionary<string, string> GetSubdivisionNames(string countryCode);
    /// <summary>
    /// Validates the subdivision code for the specified country.
    /// </summary>
    /// <param name="countryCode">The ISO country code.</param>
    /// <param name="subdivisionCode">The subdivision code to validate.</param>
    /// <returns>True if the subdivision code is valid for the country; otherwise, false.</returns>
    bool IsValidSubdivision(string countryCode, string? subdivisionCode);
}
