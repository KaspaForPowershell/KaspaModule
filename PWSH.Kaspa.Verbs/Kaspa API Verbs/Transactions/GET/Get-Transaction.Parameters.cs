using System.Management.Automation;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.Attributes;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetTransaction
    {
        [ValidateKaspaTransactionID]
        [Parameter(Mandatory = true, HelpMessage = "Specify transaction ID (hash).")]
        public string? TransactionID { get; set; }

        [ValidateKaspaBlockHash]
        [Parameter(Mandatory = false, HelpMessage = "Specify a containing block (if known) for faster lookup.")]
        public string? BlockHash { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeInputs { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeOutputs { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Fetch the TransactionInput previous outpoint details.")]
        public KaspaResolvePreviousOutpointsOption ResolvePreviousOutpoints { get; set; } = KaspaResolvePreviousOutpointsOption.No;

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
