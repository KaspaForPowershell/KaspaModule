using System.Management.Automation;
using PWSH.Kaspa.Base.Attributes;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetBlock
    {
        [ValidateKaspaBlockHash]
        [Parameter(Mandatory = true, HelpMessage = "Kaspa block ID(hash).")]
        public string? BlockID { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeTransactions { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeColor { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
