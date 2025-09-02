using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Addressing.Net.Abstractions;
using Addressing.Net.Model;

namespace Addressing.Net.Runtime.Remote;

/// <summary>
/// Provides country address metadata by fetching it at runtime from the
/// Google/Chromium dataset (<c>https://chromium-i18n.appspot.com/ssl-address/data/{CC}</c>).
/// </summary>
/// <remarks>
/// Metadata is cached in-memory per process. Once a country has been loaded,
/// subsequent calls return the cached result (including a <c>null</c> result
/// if the dataset did not provide data for that country).
/// </remarks>
public sealed class ChromiumAddressMetadataProvider : IAddressMetadataProvider, IDisposable
{
    private readonly ChromiumAddressDataClient _client;
    private readonly ConcurrentDictionary<string, CountryAddressMetadata?> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates a new provider using a default <see cref="ChromiumAddressDataClient"/>.
    /// </summary>
    public ChromiumAddressMetadataProvider()
        : this(new ChromiumAddressDataClient()) { }

    /// <summary>
    /// Creates a new provider using a custom <see cref="ChromiumAddressDataClient"/>.
    /// </summary>
    /// <param name="client">The HTTP client wrapper used to fetch data from the remote service.</param>
    public ChromiumAddressMetadataProvider(ChromiumAddressDataClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <inheritdoc />
    public void Dispose() => _client.Dispose();

    /// <inheritdoc />
    public Regex? GetPostalRegex(string countryCode)
    {
        var meta = GetOrLoad(countryCode).GetAwaiter().GetResult();
        if (meta == null || string.IsNullOrWhiteSpace(meta.PostalCodePattern)) return null;

        return new Regex("^" + meta.PostalCodePattern + "$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }

    /// <inheritdoc />
    public bool IsValidPostal(string countryCode, string? postal)
    {
        if (string.IsNullOrWhiteSpace(postal)) return false;
        var rx = GetPostalRegex(countryCode);
        return rx == null || rx.IsMatch(postal.Trim());
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetSubdivisionKeys(string countryCode)
    {
        var meta = GetOrLoad(countryCode).GetAwaiter().GetResult();
        if (meta == null || string.IsNullOrWhiteSpace(meta.SubKeys))
            return Array.Empty<string>();

        return meta.SubKeys.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> GetSubdivisionNames(string countryCode)
    {
        var meta = GetOrLoad(countryCode).GetAwaiter().GetResult();
        if (meta == null || string.IsNullOrWhiteSpace(meta.SubKeys))
            return new Dictionary<string, string>();

        var keys = meta.SubKeys.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
        var names = (meta.SubNames ?? string.Empty).Split(new[] { '~' }, StringSplitOptions.None);

        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < keys.Length; i++)
        {
            dict[keys[i]] = (i < names.Length && !string.IsNullOrWhiteSpace(names[i]))
                ? names[i]
                : keys[i];
        }
        return dict;
    }

    /// <inheritdoc />
    public bool IsValidSubdivision(string countryCode, string? subdivisionCode)
    {
        if (string.IsNullOrWhiteSpace(subdivisionCode)) return false;
        var keys = GetSubdivisionKeys(countryCode);
        foreach (var k in keys)
            if (string.Equals(k, subdivisionCode, StringComparison.OrdinalIgnoreCase))
                return true;

        return false;
    }

    // ---------- internals ----------

    private async Task<CountryAddressMetadata?> GetOrLoad(string countryCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) return null;
        var cc = countryCode.Trim().ToUpperInvariant();

        if (_cache.TryGetValue(cc, out var cached)) return cached;

        var raw = await _client.GetCountryAsync(cc, ct).ConfigureAwait(false);
        var mapped = Map(raw, cc);
        _cache[cc] = mapped;
        return mapped;
    }

    private static CountryAddressMetadata? Map(ChromiumCountryData? src, string cc)
    {
        if (src is null) return null;
        return new CountryAddressMetadata
        {
            Key = string.IsNullOrWhiteSpace(src.Key) ? cc : src.Key!.ToUpperInvariant(),
            Name = src.Name,
            PostalCodePattern = src.Zip,
            SubKeys = src.Sub_Keys,
            SubNames = src.Sub_Names
        };
    }
}
