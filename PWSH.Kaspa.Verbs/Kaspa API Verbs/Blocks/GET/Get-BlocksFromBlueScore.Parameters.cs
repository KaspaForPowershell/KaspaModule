using PWSH.Kaspa.Constants;
using System.Management.Automation;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetBlocksFromBlueScore
    {
        [ValidateRange(0, uint.MaxValue)]
        [Parameter(Mandatory = false, HelpMessage = "Only include transactions with block time before this (epoch-millis). Min/Default value : 0/43679173")]
        public uint BlueScore { get; set; } = 43679173;

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeTransactions { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Http client timeout.")]
        public ulong TimeoutSeconds { get; set; } = Globals.DEFAULT_TIMEOUT_SECONDS;

        [Parameter(Mandatory = false)]
        public SwitchParameter AsJob { get; set; }
    }
}
