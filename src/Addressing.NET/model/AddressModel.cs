namespace Addressing.Net.Model;

public sealed record AddressDto(
    string Line1,
    string? Line2,
    string City,
    string StateOrProvince, // subdivision code or name
    string PostalCode,
    string CountryCode      // ISO 3166-1 alpha-2 (NG, US, GB, …)
);
