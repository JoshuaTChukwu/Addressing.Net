namespace Addressing.Net.Model;

/// <summary>
/// Represents metadata for a country's address format, including key, name, postal code pattern, and subdivisions.
/// </summary>
public class CountryAddressMetadata
{
    /// <summary>
    /// Gets or sets the unique key identifying the country.
    /// </summary>
    public string Key { get; set; } = "";
    /// <summary>
    /// Gets or sets the display name of the country.
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Gets or sets the postal code pattern for the country, represented as a raw regular expression (no ^$).
    /// </summary>
    public string? PostalCodePattern { get; set; }  // raw regex (no ^$)
    /// <summary>
    /// Gets or sets the subdivision keys for the country, separated by "~".
    /// </summary>
    public string? SubKeys { get; set; }            // "~" separated codes
    /// <summary>
    /// Gets or sets the subdivision names for the country, separated by "~".
    /// </summary>
    public string? SubNames { get; set; }           // "~" separated names
}
