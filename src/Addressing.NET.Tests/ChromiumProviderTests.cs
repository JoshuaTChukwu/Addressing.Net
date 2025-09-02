using Addressing.Net.Runtime.Remote;
using FluentAssertions;

public class ChromiumProviderTests
{
    [Fact]
    public void US_postal_code_should_validate()
    {
        var provider = new ChromiumAddressMetadataProvider();

        // Valid US ZIP
        provider.IsValidPostal("US", "94043").Should().BeTrue();

        // Invalid US ZIP
        provider.IsValidPostal("US", "ABCDE").Should().BeFalse();

        // Subdivisions should include California (CA)
        var subs = provider.GetSubdivisionNames("US");
        subs.Should().ContainKey("CA");
    }

    [Fact]
    public void NG_postal_code_should_validate()
    {
        var provider = new ChromiumAddressMetadataProvider();

        provider.IsValidPostal("NG", "100001").Should().BeTrue();  // Lagos main
        provider.IsValidPostal("NG", "ABC123").Should().BeFalse();
    }
}
