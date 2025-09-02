using FluentValidation;
using Addressing.Net.Abstractions;
using Addressing.Net.Model;

namespace Addressing.Net.FluentValidation;

public static class FluentAddressExtensions
{
    public static IRuleBuilderOptions<T, AddressDto> ValidAddress<T>(
        this IRuleBuilder<T, AddressDto> rule,
        IAddressMetadataProvider provider)
    {
        return (IRuleBuilderOptions<T, AddressDto>)rule.Custom((addr, ctx) =>
        {
            if (addr is null) { ctx.AddFailure(ctx.PropertyPath, "Address is required."); return; }

            if (string.IsNullOrWhiteSpace(addr.Line1))
                ctx.AddFailure($"{ctx.PropertyPath}.Line1", "Line1 is required.");
            if (string.IsNullOrWhiteSpace(addr.City))
                ctx.AddFailure($"{ctx.PropertyPath}.City", "City is required.");
            if (string.IsNullOrWhiteSpace(addr.StateOrProvince))
                ctx.AddFailure($"{ctx.PropertyPath}.StateOrProvince", "State/Province is required.");
            if (string.IsNullOrWhiteSpace(addr.CountryCode) || addr.CountryCode.Length != 2)
            { ctx.AddFailure($"{ctx.PropertyPath}.CountryCode", "CountryCode must be ISO alpha-2 (e.g., NG, US)."); return; }

            if (string.IsNullOrWhiteSpace(addr.PostalCode))
                ctx.AddFailure($"{ctx.PropertyPath}.PostalCode", "PostalCode is required.");
            else if (!provider.IsValidPostal(addr.CountryCode, addr.PostalCode))
                ctx.AddFailure($"{ctx.PropertyPath}.PostalCode", "Invalid postal code.");
        });
    }
}
