using System.Management.Automation;
using PWSH.Kaspa.Base.Attributes;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetTransactionsCountForAddress
    {
        [ValidateKaspaAddress]
        [Parameter(Mandatory = true, HelpMessage = "Kaspa address as string e.g. kaspa:qqkqkzjvr7zwxxmjxjkmxxdwju9kjs6e9u82uh59z07vgaks6gg62v8707g73")]
        public string? Address { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
