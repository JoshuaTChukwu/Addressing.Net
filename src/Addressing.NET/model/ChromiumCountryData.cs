namespace Addressing.Net.Model;

public sealed class ChromiumCountryData
{
    public string? Id { get; set; }        // e.g., "data/US"
    public string? Key { get; set; }       // "US"
    public string? Name { get; set; }      // "United States"
    public string? Zip { get; set; }       // regex, e.g. "\\d{5}(-\\d{4})?"
    public string? Sub_Keys { get; set; }  // "~" separated: "CA~NY~TX~..."
    public string? Sub_Names { get; set; } // "~" separated display names
}
