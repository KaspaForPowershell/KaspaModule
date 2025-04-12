using System.Collections;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PWSH.Kaspa.Base.Attributes
{
    /// <summary>
    /// Validate if a string is a valid Kaspa block hash pattern: ^[a-f0-9]{64}$
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public sealed class ValidateKaspaBlockHashAttribute : ValidateArgumentsAttribute
    {
        private readonly Regex _regex = new(@"^[a-f0-9]{64}$", RegexOptions.Compiled);

        protected override void Validate(object arguments, EngineIntrinsics engine_intrinsics)
        {
            if (arguments is string strValue)
            {
                if (string.IsNullOrEmpty(strValue))
                    throw new ValidationMetadataException("Kaspa block hash cannot be null or empty.");

                if (!_regex.IsMatch(strValue))
                    throw new ValidationMetadataException($"Invalid Kaspa block hash: {strValue}");
            }
            else if (arguments is IEnumerable list)
            {
                foreach (var item in list ?? Array.Empty<object>()) // Prevent null reference.
                {
                    if (item is not string strItem || !this._regex.IsMatch(strItem))
                        throw new ValidationMetadataException($"Invalid Kaspa block hash in list: {item}");
                }
            }
            else throw new ValidationMetadataException("Kaspa block hash must be a string or a list of strings.");
        }
    }
}
