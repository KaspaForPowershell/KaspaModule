using System.Management.Automation;
using PWSH.Kaspa.Constants;
using PWSH.Kaspa.Verbs.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class CalculateTransactionMass 
    {
        [ValidateNotNull]
        [Parameter(Mandatory = true)]
        public IMassCalculable? Transaction { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
