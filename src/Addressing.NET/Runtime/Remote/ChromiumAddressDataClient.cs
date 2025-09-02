using System.Text.Json;
using Addressing.Net.Model;

namespace Addressing.Net.Runtime.Remote;

public sealed class ChromiumAddressDataClient : IDisposable
{
    public const string DefaultBaseUrl = "https://chromium-i18n.appspot.com/ssl-address/data";

    private readonly HttpClient _http;
    private readonly string _baseUrl;
    private bool _disposed;

    public ChromiumAddressDataClient(HttpClient? httpClient = null, string? baseUrl = null)
    {
        _http = httpClient ?? new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        _baseUrl = (baseUrl ?? DefaultBaseUrl).TrimEnd('/');
    }

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

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _http.Dispose();
    }
}
