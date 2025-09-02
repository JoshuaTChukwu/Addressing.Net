using System.Text.Json;
using Addressing.Net.Model;

namespace Addressing.Net.Runtime.Remote;

/// <summary>
/// A lightweight HTTP client for retrieving country address metadata
/// from the Google/Chromium dataset at
/// <c>https://chromium-i18n.appspot.com/ssl-address/data/{CC}</c>.
/// </summary>
public sealed class ChromiumAddressDataClient : IDisposable
{
    private const string DefaultBaseUrl = "https://chromium-i18n.appspot.com/ssl-address/data";

    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of <see cref="ChromiumAddressDataClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// Optional <see cref="HttpClient"/> to use. If <c>null</c>, a new instance
    /// with a 10-second timeout is created and owned by this client.
    /// </param>
    /// <param name="baseUrl">
    /// Optional base URL. Defaults to the official Chromium dataset endpoint.
    /// </param>
    public ChromiumAddressDataClient(HttpClient? httpClient = null, string? baseUrl = null)
    {
        _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        _baseUrl = (baseUrl ?? DefaultBaseUrl).TrimEnd('/');
    }

    /// <summary>
    /// Fetches and deserializes metadata for a given country code.
    /// </summary>
    /// <param name="countryCode">
    /// ISO 3166-1 alpha-2 country code (e.g. "US", "NG").
    /// </param>
    /// <param name="ct">
    /// Optional <see cref="CancellationToken"/> to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="ChromiumCountryData"/> object if the request succeeded,
    /// or <c>null</c> if the country code was invalid or the service returned a non-success status.
    /// </returns>
    public async Task<ChromiumCountryData?> GetCountryAsync(string countryCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(countryCode)) return null;
        var cc = countryCode.Trim().ToUpperInvariant();
        var url = $"{_baseUrl}/{cc}";

        using (var req = new HttpRequestMessage(HttpMethod.Get, url))
        using (var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false))
        {
            if (!resp.IsSuccessStatusCode) return null;

            Stream stream;
        #if NETSTANDARD2_0
            stream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
        #else
            stream = await resp.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        #endif

            return await JsonSerializer.DeserializeAsync<ChromiumCountryData>(
                stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        #if !NETSTANDARD2_0
                , ct
        #endif
            ).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Disposes the underlying <see cref="HttpClient"/> if this instance created it.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _http.Dispose();
    }
}
