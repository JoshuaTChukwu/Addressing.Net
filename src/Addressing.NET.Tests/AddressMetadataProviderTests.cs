using Addressing.Net.Runtime;
using FluentAssertions;

public class AddressMetadataProviderTests
{
    [Fact]
    public void Basic_postal_validation_should_work()
    {
        var provider = new AddressMetadataProvider();
        provider.IsValidPostal("US", "94043").Should().BeTrue();
        provider.IsValidPostal("US", "A").Should().BeFalse();
    }
}
