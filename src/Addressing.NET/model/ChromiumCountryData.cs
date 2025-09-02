namespace Addressing.Net.Model;

/// <summary>
/// Represents country address data as used in Chromium's address metadata.
/// </summary>
public sealed class ChromiumCountryData
{
    /// <summary>
    /// Gets or sets the unique identifier for the country data (e.g., "data/US").
    /// </summary>
    public string? Id { get; set; }        // e.g., "data/US"

    /// <summary>
    /// Gets or sets the country key (e.g., "US").
    /// </summary>
    public string? Key { get; set; }       // "US"

    /// <summary>
    /// Gets or sets the display name of the country (e.g., "United States").
    /// </summary>
    public string? Name { get; set; }      // "United States"

    /// <summary>
    /// Gets or sets the postal code pattern for the country, represented as a regular expression.
    /// </summary>
    public string? Zip { get; set; }       // regex, e.g. "\\d{5}(-\\d{4})?"

    /// <summary>
    /// Gets or sets the subdivision keys for the country, separated by "~".
    /// </summary>
    public string? Sub_Keys { get; set; }  // "~" separated: "CA~NY~TX~..."

    /// <summary>
    /// Gets or sets the subdivision names for the country, separated by "~".
    /// </summary>
    public string? Sub_Names { get; set; } // "~" separated display names
}
