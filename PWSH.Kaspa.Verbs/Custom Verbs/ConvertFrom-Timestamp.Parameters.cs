using System.Management.Automation;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class ConvertFromTimestamp
    {
        [Parameter(Mandatory = true)]
        public long Timestamp { get; set; }
    }
}
