#if NETSTANDARD2_0
namespace Addressing.Net.Model;

// Use a class to avoid record dependencies on old TFMs.
/// <summary>
/// Represents a standardized postal address with common international fields.
/// </summary>
public sealed class AddressDto
{
    /// <summary>
    /// Gets or sets the first address line.  
    /// Typically the street address, house number, or P.O. Box.
    /// </summary>
    public string Line1 { get; set; } = "";

    /// <summary>
    /// Gets or sets the optional second address line.  
    /// For example, apartment, suite, unit, or floor.
    /// </summary>
    public string? Line2 { get; set; }

    /// <summary>
    /// Gets or sets the city, town, or locality name.
    /// </summary>
    public string City { get; set; } = "";

    /// <summary>
    /// Gets or sets the state, province, region, or other administrative subdivision.
    /// </summary>
    public string StateOrProvince { get; set; } = "";

    /// <summary>
    /// Gets or sets the postal or ZIP code.
    /// </summary>
    public string PostalCode { get; set; } = "";

    /// <summary>
    /// Gets or sets the ISO 3166-1 alpha-2 country code (e.g. "US", "NG").
    /// </summary>
    public string CountryCode { get; set; } = "";
}
#else
namespace Addressing.Net.Model;

// Modern TFMs can use a record

/// <summary>
/// Represents a standardized postal address with common international fields.
/// </summary>
/// <param name="Line1">
/// The first address line (e.g. street address, house number, or P.O. Box).
/// </param>
/// <param name="Line2">
/// The optional second address line (e.g. apartment, suite, or floor).
/// </param>
/// <param name="City">
/// The city, town, or locality name.
/// </param>
/// <param name="StateOrProvince">
/// The state, province, region, or administrative subdivision.
/// </param>
/// <param name="PostalCode">
/// The postal or ZIP code.
/// </param>
/// <param name="CountryCode">
/// The ISO 3166-1 alpha-2 country code (e.g. "US", "NG").
/// </param>
public sealed record AddressDto(
    string Line1,
    string? Line2,
    string City,
    string StateOrProvince,
    string PostalCode,
    string CountryCode
);
#endif
