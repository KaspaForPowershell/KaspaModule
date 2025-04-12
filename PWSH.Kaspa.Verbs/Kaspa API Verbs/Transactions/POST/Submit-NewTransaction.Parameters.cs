using PWSH.Kaspa.Constants;
using System.Management.Automation;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class SubmitNewTransaction
    {
        [ValidateNotNull]
        [Parameter(Mandatory = true)]
        public TransactionRequestSchema? Transaction { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowOrphan { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Replace an existing transaction in the mempool.")]
        public SwitchParameter ReplaceByFee { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
