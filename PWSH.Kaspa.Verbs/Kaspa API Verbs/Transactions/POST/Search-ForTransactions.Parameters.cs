using System.Management.Automation;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.Attributes;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class SearchForTransactions
    {
        private static class SearchForTransactionsParameterSetName
        {
            public const string TRANSACTION_ID = "default-set";
            public const string BLUE_SCORE = "bluescore-set";
        }

/* -----------------------------------------------------------------
PARAMETERS                                                         |
----------------------------------------------------------------- */

        [ValidateKaspaTransactionID]
        [Parameter(Mandatory = true, ParameterSetName = SearchForTransactionsParameterSetName.TRANSACTION_ID, HelpMessage = "Specify transaction IDs (hash).")]
        public List<string>? TransactionIDs { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ParameterSetName = SearchForTransactionsParameterSetName.BLUE_SCORE)]
        public ulong Gte { get; set; } = 0;

        [ValidateNotNullOrEmpty]
        [Parameter(Mandatory = true, ParameterSetName = SearchForTransactionsParameterSetName.BLUE_SCORE)]
        public ulong Lt { get; set; } = 0;

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
