# Addressing.NET

**Addressing.NET** is a reusable .NET library for validating and normalizing
address data (postal codes, country subdivisions, etc.) using either:

- **Remote provider** → fetches metadata from Google’s Chromium dataset  
  (`https://chromium-i18n.appspot.com/ssl-address/data/{CC}`)
- **Embedded provider** → optional JSON files you ship with your app

It targets **.NET 9.0**, **.NET 6.0**, and **.NET Standard 2.0** for broad compatibility.

---

## ✨ Features
- Validate postal codes by country (regex-based).
- Validate subdivisions (states/provinces) by country.
- Works offline (embedded JSON) or online (Chromium dataset).
- Simple dependency injection integration.
- Optional [FluentValidation](https://docs.fluentvalidation.net/en/latest/) adapter.

---

## 📦 Installation
```bash
dotnet add package Addressing.NET
dotnet add package Addressing.NET.FluentValidation
```
## 🚀 Usage

### Configure DI
```csharp
using AddressingNet;

// For embedded JSON data
builder.Services.AddAddressingNetEmbedded();

// Or use live Chromium dataset (fetches on-demand, cached in-memory)
builder.Services.AddAddressingNetRemote();
```

### Validate addresses manually
```csharp
using AddressingNet.Abstractions;
using AddressingNet.Runtime.Remote;

var provider = new ChromiumAddressMetadataProvider();

bool isZipValid = provider.IsValidPostal("US", "94043"); // true
var states = provider.GetSubdivisionNames("US");        // { "CA" : "California", ... }
```

### With FluentValidation
```csharp
using AddressingNet.FluentValidation;
using AddressingNet.Abstractions;
using AddressingNet.Model;
using FluentValidation;

public record RegisterDto(AddressDto Address);

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator(IAddressMetadataProvider provider)
    {
        RuleFor(x => x.Address).NotNull().ValidAddress(provider);
    }
}
```

## 🧪 Running tests
```csharp
dotnet build Addressing.NET.sln
dotnet test Addressing.NET.sln
```

## 📄 License

MIT © 2025 Joshua Tochukwu Chukwu
