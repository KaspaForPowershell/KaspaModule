using System.Management.Automation;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Convert date to timespan.
    /// </summary>
    [Cmdlet(KaspaVerbNames.ConvertTo, "Timestamp")]
    [OutputType(typeof(long))]
    public sealed partial class ConvertToTimestamp : KaspaPSCmdlet
    {
        protected override void BeginProcessing()
        {
            // Ensure we account for months that has less than 31 days and leap days.
            var maxDays = DateTime.DaysInMonth((int)Year, (int)Month);
            if (Day > maxDays)
            {
                ThrowTerminatingError(new ErrorRecord(new ArgumentOutOfRangeException(nameof(Day), $"The {Month} month of {Year} year only has {maxDays} days."),
                    "InvalidDay",
                    ErrorCategory.InvalidArgument,
                    Day
                ));
            }
        }

        protected override void EndProcessing()
        {
            var date = new DateTime((int)Year, (int)Month, (int)Day, (int)Hour, (int)Minute, (int)Second, (int)Millisecond, DateKind);
            var output = ((DateTimeOffset)date).ToUnixTimeMilliseconds();

            WriteObject(output);
        }
    }
}
