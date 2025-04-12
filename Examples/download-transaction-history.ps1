
<#
    .SYNOPSIS
        Concurrently retrieves transaction history for a Kaspa address.

    .DESCRIPTION
        This script fetches the complete transaction history for a specified Kaspa address by making
        concurrent API calls in batches. It handles pagination automatically and combines all results
        into a single collection. The script supports customizing fields returned, limiting concurrency,
        and resolving previous outpoints for comprehensive transaction data.

    .PARAMETER Address
        The Kaspa address to query transactions for. Must be a valid Kaspa address format.

    .PARAMETER ConcurrencyLimit
        The maximum number of concurrent API requests to make.
        Range: 1 to maximum unsigned integer value.
        Default: 5
        
        Higher values may improve performance but could potentially overload the API service.

    .PARAMETER Fields
        Specific fields to retrieve for each transaction. Leave empty for all fields.
        Default: "" (empty string, returns all fields)

    .PARAMETER ResolvePreviousOutpoints
        Values: No, Light, Full
        Default: No
        - No: No resolution of previous outpoints
        - Light: Basic resolution with minimal transaction details
        - Full: Complete resolution with all transaction details for previous outpoints

    .PARAMETER CleanConsole
        An optional switch to clear the console screen before processing the addresses. 
        If this switch is provided, the console will be cleared at the beginning of the script execution.

    .EXAMPLE
        PS> .\download-transaction-history.ps1
        Retrieves all transactions for the default address with default settings.

    .EXAMPLE
        PS> .\download-transaction-history.ps1 -Address "kaspa:qqscm7geuuc26ffneeyslsfcytg0vzf9848slkxchzdkgx3mn5mdx4dcavk2r" -ConcurrencyLimit 10
        Retrieves all transactions for the specified address with increased concurrency.

    .EXAMPLE
        PS> .\download-transaction-history.ps1 -Address "kaspa:qqscm7geuuc26ffneeyslsfcytg0vzf9848slkxchzdkgx3mn5mdx4dcavk2r" -Fields "subnetwork_id,transaction_id,block_time"
        Retrieves only specific fields (subnetwork_id, transaction_id, block_time) for all transactions.

    .OUTPUTS
        Returns a custom PSCustomObject object with the following properties:
        - TransactionsCount: Total number of transactions retrieved
        - Transactions: Array of transaction objects
        - FailedOffsets: List of any offsets that failed during retrieval

    .NOTES
        The script uses PowerShell jobs for parallel processing to improve performance.
#>

param
(
    [PWSH.Kaspa.Base.Attributes.ValidateKaspaAddress()]
    [Parameter(Mandatory=$true)]
    [string] $Address,

    [ValidateRange(1, [uint]::MaxValue)]
    [Parameter(Mandatory=$false)]
    [uint] $ConcurrencyLimit = 5,

    [ValidateNotNullOrEmpty()]
    [Parameter(Mandatory=$false)]
    [string] $Fields = "",

    [Parameter(Mandatory=$false)]
    [PWSH.Kaspa.Base.KaspaResolvePreviousOutpointsOption] $ResolvePreviousOutpoints = [PWSH.Kaspa.Base.KaspaResolvePreviousOutpointsOption]::No,

    [Parameter(Mandatory=$false)]
    [switch] $CleanConsole
)

<# -----------------------------------------------------------------
DEFAULTS                                                           |
----------------------------------------------------------------- #>

# Clear console if the CleanConsole switch is provided.
if ($CleanConsole.IsPresent) { Clear-Host }

# Maximum transactions per job. It's API limit, so we set it to be readonly  https://tommymaynard.com/read-only-and-constant-variables/
New-Variable -Name "BATCH_SIZE" -Value 500 -Option ReadOnly -Scope Script

<# -----------------------------------------------------------------
MAIN                                                               |
----------------------------------------------------------------- #>

# First check if the address has any transactions at all to avoid unnecessary processing.
$transactionsCount = Get-TransactionsCountForAddress -Address $Address
if (-not($transactionsCount.Total -gt 0)) { return $null }

Write-Host "Starting parallel transaction retrieval mode with $($ConcurrencyLimit) concurrent job(s)..." -ForegroundColor Cyan

# Initialize collections to store our results and track failures.
$allResults = @()           # Stores all successfully retrieved transactions.
$failedOffsets = @()        # Tracks any failed batch retrievals by their offset.
$page = 0                   # Starting page (used to calculate offsets).
$shouldContinue = $true     # Flag to control the pagination loop.

# Main pagination loop - continues until we've retrieved all transactions or encountered a stopping condition.
while ($shouldContinue)
{
    Write-Host "`nStarting batch at page $($page)..." -ForegroundColor Magenta

    $tasks = @()        # Collection to store the async job objects.
    $offsets = @{}      # Dictionary to map job IDs to their corresponding offsets.

     # Start a batch of concurrent jobs based on the concurrency limit.
    for ($i = 0; $i -lt $ConcurrencyLimit; $i++) 
    {
        # Calculate the starting offset for this job.
        $currentOffset = ($page + $i) * $BATCH_SIZE
        Write-Host "  Starting job for transactions $($currentOffset) to $($currentOffset + $BATCH_SIZE - 1)..." -ForegroundColor Blue

        # Create the appropriate job based on whether fields are specified.
        $job = if ($Fields -eq [string]::Empty) { Get-FullTransactionsForAddress -Address $Address -Limit $BATCH_SIZE -ResolvePreviousOutpoints $ResolvePreviousOutpoints -Offset $currentOffset -AsJob }
        else { Get-FullTransactionsForAddress -Address $Address -Limit $BATCH_SIZE -ResolvePreviousOutpoints $ResolvePreviousOutpoints -Offset $currentOffset -Fields $Fields -AsJob }

        # Store the job and its corresponding offset for later processing.
        $tasks += $job
        $offsets[$job.Id] = $currentOffset
    }

    Write-Host "Waiting for $($tasks.Count) job(s) to complete..." -ForegroundColor Cyan
    $null = $tasks | Wait-Job   # Wait for all jobs in this batch to complete before processing their results.

    $currentPage = $page

    # Process the results from each job.
    foreach ($job in $tasks) 
    {
        # Handle failed jobs by recording their offsets for potential retry.
        if ($job.State -eq 'Failed') 
        { 
            Write-Warning "Job $($job.Id) failed: $($job.Error)"
            $failedOffsets += $offsets[$job.Id]
            Remove-Job -Id $job.Id -Force
            continue
        }

        Write-Host "Processing results from job ID $($job.Id) (page $($currentPage))..." -ForegroundColor Blue
        $pageResult = Receive-Job -Job $job
        Remove-Job -Job $job -Force

        # Process successful results and check if we've reached the end of the data.
        if ($null -ne $pageResult) 
        {
            Write-Host "  Retrieved $($pageResult.Count) transactions" -ForegroundColor Green
            $allResults += $pageResult
    
            # If we get fewer transactions than the batch size, we've reached the end.
            if ($pageResult.Count -lt $BATCH_SIZE) 
            {
                Write-Host "  Less than $($BATCH_SIZE) transactions returned. Assuming end of data." -ForegroundColor Yellow
                $shouldContinue = $false
                break
            }
        }

        $currentPage++
    }

    # Advance to the next set of pages based on our concurrency limit.
    $page = $page + $ConcurrencyLimit;

    # Brief delay to prevent overwhelming the API server with requests.
    # Adjust this value based on the API's rate limiting policies.
    Start-Sleep -Seconds 1
}

<# -----------------------------------------------------------------
OUTPUT                                                             |
----------------------------------------------------------------- #>

return [PSCustomObject]@{
    TransactionsCount = $allResults.Count   # Total number of transactions successfully retrieved.
    Transactions = $allResults              # The actual transaction data.
    FailedOffsets = $failedOffsets          # List of offsets that failed (useful for retries).
}