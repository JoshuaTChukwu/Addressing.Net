using Addressing.Net.Abstractions;
using Addressing.Net.Runtime;
using Addressing.Net.Runtime.Remote;
using Microsoft.Extensions.DependencyInjection;

namespace Addressing.Net;

/// <summary>
/// Extension methods for registering Addressing.NET providers with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="AddressMetadataProvider"/> as a singleton <see cref="IAddressMetadataProvider"/>.
    /// </summary>
    /// <remarks>
    /// Use this if you ship JSON country metadata files embedded in your package or application.
    /// The <see cref="AddressMetadataProvider"/> reads from <c>data/*.json</c> resources bundled in the assembly.
    /// </remarks>
    /// <param name="services">The DI service collection.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddAddressingNetEmbedded(this IServiceCollection services)
        => services.AddSingleton<IAddressMetadataProvider, AddressMetadataProvider>();

    /// <summary>
    /// Registers <see cref="ChromiumAddressMetadataProvider"/> as a singleton <see cref="IAddressMetadataProvider"/>.
    /// </summary>
    /// <remarks>
    /// Use this variant to fetch live metadata from the Google/Chromium dataset
    /// (<c>https://chromium-i18n.appspot.com/ssl-address/data/{CC}</c>).
    /// Data is cached in-memory per process to avoid repeated requests.
    /// </remarks>
    /// <param name="services">The DI service collection.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddAddressingNetRemote(this IServiceCollection services)
        => services.AddSingleton<IAddressMetadataProvider, ChromiumAddressMetadataProvider>();
}
