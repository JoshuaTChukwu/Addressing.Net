#if NETSTANDARD2_0
namespace Addressing.Net.Model;

// Use a class to avoid record dependencies on old TFMs.
public sealed class AddressDto
{
    public string Line1 { get; set; } = "";
    public string? Line2 { get; set; }
    public string City { get; set; } = "";
    public string StateOrProvince { get; set; } = "";
    public string PostalCode { get; set; } = "";
    public string CountryCode { get; set; } = ""; // ISO alpha-2
}
#else
namespace Addressing.Net.Model;

// Modern TFMs can use a record
public sealed record AddressDto(
    string Line1,
    string? Line2,
    string City,
    string StateOrProvince,
    string PostalCode,
    string CountryCode
);
#endif
