<#
    .SYNOPSIS
        Retrieves and categorizes transactions for a specified Kaspa address.

    .DESCRIPTION
        This script connects to the Kaspa network and retrieves transactions for a given address.
        Transactions are categorized into mining transactions and regular transactions.
        The script handles pagination for addresses with large transaction histories and 
        provides information about the oldest and newest transactions.

    .PARAMETER Address
        The Kaspa address to retrieve transactions for. This parameter is validated to ensure
        it conforms to the Kaspa address format.

    .PARAMETER FullTransactions
        If specified, retrieves the full transaction objects from the API. 
        If omitted, a lightweight transaction view is used (only subnetwork ID, block time, and transaction ID).
        
    .PARAMETER CleanConsole
        An optional switch to clear the console screen before processing the addresses. 
        If this switch is provided, the console will be cleared at the beginning of the script execution.

    .EXAMPLE
        PS> .\filter-transactions.ps1 -Address "kaspa:qqscm7geuuc26ffneeyslsfcytg0vzf9848slkxchzdkgx3mn5mdx4dcavk2r" -FullTransactions
        
        Retrieves transactions for the specified address with all fields included.

    .EXAMPLE
        PS> .\filter-transactions.ps1 -Address "kaspa:qqscm7geuuc26ffneeyslsfcytg0vzf9848slkxchzdkgx3mn5mdx4dcavk2r" -CleanConsole
        
        Clears the console before retrieving transactions for the specified address.

    .OUTPUTS
        Returns a custom object containing two properties:
        - MinerTransactions: An array of transactions related to mining activities
        - OtherTransactions: An array of all other transactions
#>

param 
(
    [PWSH.Kaspa.Base.Attributes.ValidateKaspaAddress()]
    [Parameter(Mandatory=$true)]
    [string] $Address,

    [Parameter(Mandatory=$false)]
    [switch] $FullTransactions,

    [Parameter(Mandatory=$false)]
    [switch] $CleanConsole
)

<# -----------------------------------------------------------------
DEFAULTS                                                           |
----------------------------------------------------------------- #>

# Clear console if the CleanConsole switch is provided.
if ($CleanConsole.IsPresent) { Clear-Host }

<# -----------------------------------------------------------------
MAIN                                                               |
----------------------------------------------------------------- #>

# Leverage the download-transaction-history.ps1 script to retrieve transaction data.
# This approach demonstrates modular script design - reusing existing functionality instead of duplicating code.
$result = if (-not $FullTransactions.IsPresent) { ./download-transaction-history.ps1 -Address $Address -ResolvePreviousOutpoints No -Fields "subnetwork_id,block_time,transaction_id" }
else { ./download-transaction-history.ps1 -Address $Address -ResolvePreviousOutpoints No }

# Handle the case when no transactions are found.
if (($null -eq $result) -or ($result.TransactionsCount -eq 0)) { return $null }

Write-Host "  Processing $($result.TransactionsCount) transactions..." -ForegroundColor Cyan

# Initialize arrays to store the categorized transactions.
$minerTxs = @()     # Will hold mining-related transactions.
$otherTxs = @()     # Will hold all other transactions.

# Categorize each transaction based on its subnetwork ID.
foreach($tx in $result.Transactions)
{
    if ($tx.SubnetworkID -eq "0100000000000000000000000000000000000000") { $minerTxs += $tx } # This is mining subnetwork https://github.com/kaspa-ng/kaspa-rest-server/pull/63/files
    else { $otherTxs += $tx }
}

Write-Host ("  Found {0} mining transactions and {1} other transactions" -f $minerTxs.Count, $otherTxs.Count)-ForegroundColor Cyan

# Sort all transactions by block time to find the oldest and newest.
$sorted = $result.Transactions | Sort-Object -Property BlockTime

# Extract and display information about the oldest transaction.
$oldestTimestamp = ($sorted | Select-Object -First 1).BlockTime
$oldestDate = ConvertFrom-Timestamp -Timestamp $oldestTimestamp
Write-Host ("  Oldest transaction: {0} ({1})" -f $oldestDate.LocalDateTime, $oldestTimestamp) -ForegroundColor DarkCyan

# Extract and display information about the newest transaction.
$newestTimestamp = ($sorted | Select-Object -Last 1).BlockTime
$newestDate = ConvertFrom-Timestamp -Timestamp $newestTimestamp
Write-Host ("  Newest transaction: {0} ({1})" -f $newestDate.LocalDateTime, $newestTimestamp) -ForegroundColor DarkCyan

<# -----------------------------------------------------------------
OUTPUT                                                             |
----------------------------------------------------------------- #>

return [PSCustomObject]@{
    MinerTransactions = $minerTxs   # Transactions related to mining (identified by subnetwork ID).
    OtherTransactions = $otherTxs   # All non-mining transactions.
}