using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

const string BaseUrl = "https://chromium-i18n.appspot.com/ssl-address/data";
var allIso2 = new[]
{
  // ISO 3166-1 alpha-2 (core set; extend as needed)
  "AD","AE","AF","AG","AI","AL","AM","AO","AR","AS","AT","AU","AW","AX","AZ",
  "BA","BB","BD","BE","BF","BG","BH","BI","BJ","BL","BM","BN","BO","BQ","BR","BS","BT","BV","BW","BY","BZ",
  "CA","CC","CD","CF","CG","CH","CI","CK","CL","CM","CN","CO","CR","CU","CV","CW","CX","CY","CZ",
  "DE","DJ","DK","DM","DO","DZ",
  "EC","EE","EG","EH","ER","ES","ET",
  "FI","FJ","FK","FM","FO","FR",
  "GA","GB","GD","GE","GF","GG","GH","GI","GL","GM","GN","GP","GQ","GR","GS","GT","GU","GW","GY",
  "HK","HM","HN","HR","HT","HU",
  "ID","IE","IL","IM","IN","IO","IQ","IR","IS","IT",
  "JE","JM","JO","JP",
  "KE","KG","KH","KI","KM","KN","KP","KR","KW","KY","KZ",
  "LA","LB","LC","LI","LK","LR","LS","LT","LU","LV","LY",
  "MA","MC","MD","ME","MF","MG","MH","MK","ML","MM","MN","MO","MP","MQ","MR","MS","MT","MU","MV","MW","MX","MY","MZ",
  "NA","NC","NE","NF","NG","NI","NL","NO","NP","NR","NU","NZ",
  "OM",
  "PA","PE","PF","PG","PH","PK","PL","PM","PN","PR","PS","PT","PW","PY",
  "QA",
  "RE","RO","RS","RU","RW",
  "SA","SB","SC","SD","SE","SG","SH","SI","SJ","SK","SL","SM","SN","SO","SR","SS","ST","SV","SX","SY","SZ",
  "TC","TD","TF","TG","TH","TJ","TK","TL","TM","TN","TO","TR","TT","TV","TW","TZ",
  "UA","UG","UM","US","UY","UZ",
  "VA","VC","VE","VG","VI","VN","VU",
  "WF","WS",
  "YE","YT",
  "ZA","ZM","ZW"
};

// CLI args: --out <dir> [--only US,NG,GB] [--overwrite]
string outDir = GetArg("--out") ?? "Addressing.NET/data";
var only = (GetArg("--only") ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                   .Select(x => x.ToUpperInvariant()).ToHashSet();
bool overwrite = HasFlag("--overwrite");

Directory.CreateDirectory(outDir);

using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };

int ok = 0, skip = 0, err = 0;
foreach (var cc in allIso2)
{
    if (only.Count > 0 && !only.Contains(cc)) { skip++; continue; }

    string path = Path.Combine(outDir, $"{cc}.json");
    if (!overwrite && File.Exists(path)) { Console.WriteLine($"skip {cc} (exists)"); skip++; continue; }

    try
    {
        var url = $"{BaseUrl}/{cc}";
        using var resp = await http.GetAsync(url);
        if (resp.StatusCode == HttpStatusCode.NotFound)
        {
            Console.WriteLine($"warn {cc}: 404 (no dataset)");
            skip++;
            continue;
        }
        resp.EnsureSuccessStatusCode();

        await using var stream = await resp.Content.ReadAsStreamAsync();
        var chromium = await JsonSerializer.DeserializeAsync<ChromiumCountryData>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (chromium is null)
        {
            Console.WriteLine($"warn {cc}: empty body");
            skip++;
            continue;
        }

        var mapped = new CountryAddressMetadata
        {
            Key = string.IsNullOrWhiteSpace(chromium.Key) ? cc : chromium.Key!.ToUpperInvariant(),
            Name = chromium.Name ?? cc,
            PostalCodePattern = chromium.Zip,       // raw regex
            SubKeys = chromium.Sub_Keys,            // "~" separated
            SubNames = chromium.Sub_Names
        };

        // Serialize using your library's shape
        var json = JsonSerializer.Serialize(mapped, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        await File.WriteAllTextAsync(path, json);
        Console.WriteLine($"ok   {cc}");
        ok++;
        await Task.Delay(50); // be gentle
    }
    catch (Exception ex)
    {
        Console.WriteLine($"fail {cc}: {ex.Message}");
        err++;
    }
}

Console.WriteLine($"\nDone. ok={ok} skip={skip} err={err}. Output => {Path.GetFullPath(outDir)}");

// ------------- helpers & models -------------
static string? GetArg(string name)
{
    var args = Environment.GetCommandLineArgs();
    for (int i = 0; i < args.Length - 1; i++)
        if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            return args[i + 1];
    return null;
}
static bool HasFlag(string name)
{
    var args = Environment.GetCommandLineArgs();
    return args.Any(a => string.Equals(a, name, StringComparison.OrdinalIgnoreCase));
}

sealed class ChromiumCountryData
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Zip { get; set; }
    public string? Sub_Keys { get; set; }
    public string? Sub_Names { get; set; }
}

sealed class CountryAddressMetadata
{
    public string Key { get; set; } = "";
    public string? Name { get; set; }
    public string? PostalCodePattern { get; set; }
    public string? SubKeys { get; set; }
    public string? SubNames { get; set; }
}
