using System.Management.Automation;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetCirculatingCoins
    {
        [Parameter(Mandatory = false)]
        public SwitchParameter InBillion { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
