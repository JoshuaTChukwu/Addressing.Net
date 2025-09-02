namespace Addressing.Net.Model;

public class CountryAddressMetadata
{
    public string Key { get; set; } = "";
    public string? Name { get; set; }
    public string? PostalCodePattern { get; set; }  // raw regex (no ^$)
    public string? SubKeys { get; set; }            // "~" separated codes
    public string? SubNames { get; set; }           // "~" separated names
}
