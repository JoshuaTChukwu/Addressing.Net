using Addressing.Net.Abstractions;
using Addressing.Net.Runtime;
using Addressing.Net.Runtime.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Addressing.Net;

public static class ServiceCollectionExtensions
{
    /// Use this if you ship JSON in your package/app (AddressMetadataProvider reads embedded files).
    public static IServiceCollection AddAddressingNetEmbedded(this IServiceCollection services)
        => services.AddSingleton<IAddressMetadataProvider, AddressMetadataProvider>();

    /// Use this to fetch Google/Chromium data at runtime with in-memory caching.
    public static IServiceCollection AddAddressingNetRemote(this IServiceCollection services)
        => services.AddSingleton<IAddressMetadataProvider, ChromiumAddressMetadataProvider>();
}
