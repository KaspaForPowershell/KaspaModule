using System.Management.Automation;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.Attributes;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetFullTransactionsForAddress
    {
        [ValidateKaspaAddress]
        [Parameter(Mandatory = true, HelpMessage = "Kaspa address as string e.g. kaspa:qqkqkzjvr7zwxxmjxjkmxxdwju9kjs6e9u82uh59z07vgaks6gg62v8707g73")]
        public string? Address { get; set; }

        [ValidateRange(1, 500)]
        [Parameter(Mandatory = false, HelpMessage = "The number of records to get. Min/Max/Default value: 1/500/50")]
        public uint Limit { get; set; } = 50;

        [ValidateRange(0, uint.MaxValue)]
        [Parameter(Mandatory = false, HelpMessage = "The offset from which to get records. Min/Default value : 0/0")]
        public uint Offset { get; set; } = 0;

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = false)]
        public string Fields { get; set; } = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "Fetch the TransactionInput previous outpoint details.")]
        public KaspaResolvePreviousOutpointsOption ResolvePreviousOutpoints { get; set; } = KaspaResolvePreviousOutpointsOption.No;

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
