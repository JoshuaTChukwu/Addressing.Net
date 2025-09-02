using FluentValidation;
using Addressing.Net.Abstractions;
using Addressing.Net.Model;

namespace Addressing.Net.FluentValidation;

/// <summary>
/// Provides FluentValidation extensions for validating <see cref="AddressDto"/> values
/// against country metadata supplied by an <see cref="IAddressMetadataProvider"/>.
/// </summary>
public static class FluentAddressExtensions
{
    /// <summary>
    /// Adds address validation rules for an <see cref="AddressDto"/>.
    /// </summary>
    /// <typeparam name="T">The type being validated.</typeparam>
    /// <param name="rule">The rule builder for the <see cref="AddressDto"/> property.</param>
    /// <param name="provider">
    /// The <see cref="IAddressMetadataProvider"/> used to validate postal codes
    /// and (optionally) subdivision rules.
    /// </param>
    /// <returns>
    /// A rule builder options object that can be chained with additional configuration.
    /// </returns>
    /// <remarks>
    /// The following checks are enforced:
    /// <list type="bullet">
    ///   <item><description><c>Line1</c>, <c>City</c>, <c>StateOrProvince</c>, and <c>PostalCode</c> must be non-empty.</description></item>
    ///   <item><description><c>CountryCode</c> must be exactly 2 characters (ISO alpha-2).</description></item>
    ///   <item><description><c>PostalCode</c> must match the regex defined for the given country (if available).</description></item>
    /// </list>
    /// </remarks>
    public static IRuleBuilderOptions<T, AddressDto> ValidAddress<T>(
        this IRuleBuilder<T, AddressDto> rule,
        IAddressMetadataProvider provider)
    {
        return (IRuleBuilderOptions<T, AddressDto>)rule.Custom((addr, ctx) =>
        {
            if (addr is null)
            {
                ctx.AddFailure(ctx.PropertyPath, "Address is required.");
                return;
            }

            if (string.IsNullOrWhiteSpace(addr.Line1))
                ctx.AddFailure($"{ctx.PropertyPath}.Line1", "Line1 is required.");
            if (string.IsNullOrWhiteSpace(addr.City))
                ctx.AddFailure($"{ctx.PropertyPath}.City", "City is required.");
            if (string.IsNullOrWhiteSpace(addr.StateOrProvince))
                ctx.AddFailure($"{ctx.PropertyPath}.StateOrProvince", "State/Province is required.");

            if (string.IsNullOrWhiteSpace(addr.CountryCode) || addr.CountryCode.Length != 2)
            {
                ctx.AddFailure($"{ctx.PropertyPath}.CountryCode", "CountryCode must be ISO alpha-2 (e.g., NG, US).");
                return;
            }

            if (string.IsNullOrWhiteSpace(addr.PostalCode))
                ctx.AddFailure($"{ctx.PropertyPath}.PostalCode", "PostalCode is required.");
            else if (!provider.IsValidPostal(addr.CountryCode, addr.PostalCode))
                ctx.AddFailure($"{ctx.PropertyPath}.PostalCode", "Invalid postal code.");
        });
    }
}
