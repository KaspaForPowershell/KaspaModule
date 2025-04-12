using System.Management.Automation;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class ConvertToTimestamp
    {
/* -----------------------------------------------------------------
PARAMETERS                                                         |
----------------------------------------------------------------- */

        [ValidateRange(1, 31)]
        [Parameter(Mandatory = true)]
        public uint Day { get; set; }

        [ValidateRange(1, 12)]
        [Parameter(Mandatory = true)]
        public uint Month { get; set; }

        [Parameter(Mandatory = true)]
        public uint Year { get; set; }

        [ValidateRange(0, 23)]
        [Parameter(Mandatory = false)]
        public uint Hour { get; set; } = 0;

        [ValidateRange(0, 59)]
        [Parameter(Mandatory = false)]
        public uint Minute { get; set; } = 0;

        [ValidateRange(0, 59)]
        [Parameter(Mandatory = false)]
        public uint Second { get; set; } = 0;

        [ValidateRange(0, 999)]
        [Parameter(Mandatory = false)]
        public uint Millisecond { get; set; } = 0;

        [Parameter(Mandatory = false)]
        public DateTimeKind DateKind { get; set; } = DateTimeKind.Utc;
    }
}
