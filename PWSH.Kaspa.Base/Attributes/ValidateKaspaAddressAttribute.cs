using System.Collections;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PWSH.Kaspa.Base.Attributes
{
    /// <summary>
    /// Validate if a string is a valid Kaspa address pattern: ^kaspa:[a-z0-9]{61,63}$
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class ValidateKaspaAddressAttribute : ValidateArgumentsAttribute
    {
        private readonly Regex _regex = new(@"^kaspa:[a-z0-9]{61,63}$", RegexOptions.Compiled);

        protected override void Validate(object arguments, EngineIntrinsics engine_intrinsics)
        {
            if (arguments is string strValue)
            {
                if (string.IsNullOrEmpty(strValue))
                    throw new ValidationMetadataException("Kaspa address cannot be null or empty.");

                if (!_regex.IsMatch(strValue))
                    throw new ValidationMetadataException($"Invalid Kaspa address: {strValue}");
            }
            else if (arguments is IEnumerable list)
            {
                foreach (var item in list ?? Array.Empty<object>()) // Prevent null reference.
                {
                    if (item is not string strItem || !this._regex.IsMatch(strItem))
                        throw new ValidationMetadataException($"Invalid Kaspa address in list: {item}");
                }
            }
            else throw new ValidationMetadataException("Kaspa address must be a string or a list of strings.");
        }
    }
}
