using Addressing.Net.Runtime;
using FluentAssertions;

public class EmbeddedProviderTests
{
    [Fact]
    public void US_postal_code_should_validate()
    {
        var provider = new AddressMetadataProvider();

        provider.IsValidPostal("US", "94043").Should().BeTrue();
        provider.IsValidPostal("US", "ABCDE").Should().BeFalse();

        var subs = provider.GetSubdivisionNames("US");
        subs.Should().ContainKey("CA");
    }

    [Fact]
    public void NG_postal_code_should_validate()
    {
        var provider = new AddressMetadataProvider();

        provider.IsValidPostal("NG", "100001").Should().BeTrue();
        provider.IsValidPostal("NG", "ABC123").Should().BeFalse();
    }
}
