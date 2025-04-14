<#
    .SYNOPSIS
        Parallel transaction retriever for a Kaspa address using background jobs.

    .DESCRIPTION
        This script fetches the complete transaction history for a specified Kaspa address by making
        concurrent API calls in batches. It handles pagination automatically and combines all results
        into a single collection. The script supports customizing fields returned, limiting concurrency,
        and resolving previous outpoints for comprehensive transaction data.
        It also includes intelligent retry logic for failed requests and optional console cleaning for better readability.

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
        Determines whether to fetch previous outpoint data and how detailed that resolution should be.
        Valid values:
        - No   : No resolution (default)
        - Light: Minimal information about previous outpoints
        - Full : Full transaction detail resolution of each previous outpoint

    .PARAMETER MaxFailedTries
        The maximum number of retry attempts for addresses from failed jobs before abandoning processing. Provides resilience against temporary network or API issues. Defaults to 3 if not specified.

    .PARAMETER CleanConsole
        An optional switch to clear the console screen before processing the addresses. 
        If this switch is provided, the console will be cleared at the beginning of the script execution.

    .EXAMPLE
        PS> .\download-transaction-history.ps1 -Address "kaspa:qqscm7geuuc26ffneeyslsfcytg0vzf9848slkxchzdkgx3mn5mdx4dcavk2r" -ConcurrencyLimit 10
        Retrieves all transactions for the specified address with increased concurrency.

    .EXAMPLE
        PS> .\download-transaction-history.ps1 -Address "kaspa:qqscm7geuuc26ffneeyslsfcytg0vzf9848slkxchzdkgx3mn5mdx4dcavk2r" -Fields "subnetwork_id,transaction_id,block_time"
        Retrieves only specific fields (subnetwork_id, transaction_id, block_time) for all transactions.
        
    .OUTPUTS
        Returns a custom PSCustomObject object with the following properties:
        - TransactionsCount: Number of transactions retrieved
        - Transactions: List of retrieved transaction data
        - FailedOffsets: Any offsets that failed retrieval after retries
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

    [ValidateRange(0, [uint]::MaxValue)]
    [Parameter(Mandatory=$false)]
    [uint] $MaxFailedTries = 3,

    [Parameter(Mandatory=$false)]
    [switch] $CleanConsole
)

<# -----------------------------------------------------------------
DEFAULTS                                                           |
----------------------------------------------------------------- #>

# Clear console if the CleanConsole switch is provided.
if ($CleanConsole.IsPresent) { Clear-Host }

# Set maximum transactions per API call (fixed by API limits), so we set it to be readonly  https://tommymaynard.com/read-only-and-constant-variables/
New-Variable -Name "BATCH_SIZE" -Value 500 -Option ReadOnly -Scope Script

<# -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- #>

function Start-TransactionRetrievalJobsByOffsets
{
    <#
        .SYNOPSIS
            Starts parallel background jobs to retrieve Kaspa transactions in batches.

        .DESCRIPTION
            This helper function launches background jobs to fetch transaction data for a specific address. Each job processes a separate offset, and the function respects a maximum concurrency limit.

        .PARAMETER Address
            Look at main script parameters for parameter explanation.

        .PARAMETER ConcurrencyLimit
            Look at main script parameters for parameter explanation.

        .PARAMETER Fields
            Look at main script parameters for parameter explanation.

        .PARAMETER ResolvePreviousOutpoints
            Look at main script parameters for parameter explanation.

        .PARAMETER PendingOffsets
            A queue of offsets (page indices) to use for each job.

        .OUTPUTS
            Returns a custom PSCustomObject object with the following properties:
            - Jobs: List of job objects started
            - Offsets: Mapping of job IDs to the offsets they processed
    #>

    param
    (
        [Parameter(Mandatory=$true)]
        [string] $Address,

        [Parameter(Mandatory=$true)]
        [uint] $ConcurrencyLimit,

        [AllowEmptyString()]
        [Parameter(Mandatory=$true)]
        [string] $Fields,

        [Parameter(Mandatory=$true)]
        [PWSH.Kaspa.Base.KaspaResolvePreviousOutpointsOption] $ResolvePreviousOutpoints,

        [Parameter(Mandatory=$true)]
        [System.Collections.Generic.Queue[uint]] $PendingOffsets
    )

    $jobs = @()         # Collection to store the async job objects.
    $offsets = @{}      # Dictionary to map job IDs to their corresponding offsets.

    for ($i = 0; $i -lt $ConcurrencyLimit -and $PendingOffsets.Count -gt 0; $i++) 
    {
        $currentOffset = $PendingOffsets.Dequeue()
        Write-Host "  Starting job for transactions $($currentOffset) to $($currentOffset + $BATCH_SIZE - 1)..." -ForegroundColor Blue

        # Create the appropriate job based on whether fields are specified.
        $job = if ($Fields -eq [string]::Empty) { Get-FullTransactionsForAddress -Address $Address -Limit $BATCH_SIZE -ResolvePreviousOutpoints $ResolvePreviousOutpoints -Offset $currentOffset -AsJob }
        else { Get-FullTransactionsForAddress -Address $Address -Limit $BATCH_SIZE -ResolvePreviousOutpoints $ResolvePreviousOutpoints -Offset $currentOffset -Fields $Fields -AsJob }

        # Store the job and its corresponding offset for later processing.
        $jobs += $job
        $offsets[$job.Id] = $currentOffset
    }

    return [PSCustomObject]@{
        Jobs = $jobs
        Offsets = $offsets
    }
}

function Resolve-TransactionRetrievalResult
{
    <#
        .SYNOPSIS
        Processes the results of transaction retrieval jobs.

        .DESCRIPTION
        Waits for each job to complete, collects results, detects end-of-data scenarios, and gathers failed offsets for retries.

        .PARAMETER RetrievalResult
            The object containing jobs and their corresponding offsets as returned from Start-TransactionRetrievalJobsByOffsets.

        .PARAMETER Address
            Look at main script parameters for parameter explanation.

        .OUTPUTS
            Returns a custom PSCustomObject object with the following properties:
            - EndOfData: Boolean indicating if the end of available data was reached
            - Transactions: Aggregated list of retrieved transactions
            - FailedOffsets: List of offsets where jobs failed and need retry
    #>

    param
    (
        [Parameter(Mandatory=$true)]
        [PSCustomObject] $RetrievalResult,

        [Parameter(Mandatory=$true)]
        [string] $Address
    )

    $endOfData = $false
    $returnResults = @()
    $failedOffsets = @()

    foreach ($job in $RetrievalResult.Jobs) 
    {
        # Handle failed jobs by recording their offsets for potential retry.
        if ($job.State -eq 'Failed') 
        { 
            Write-Warning "Job $($job.Id) failed: $($job.Error)"
            $failedOffset = $RetrievalResult.Offsets[$job.Id]
            $failedOffsets += $failedOffset 
            Write-Host "  Recorded failed offset: $($failedOffset)" -ForegroundColor Red
            Remove-Job -Id $job.Id -Force
            continue
        }

        Write-Host "Processing results from job ID $($job.Id)..." -ForegroundColor Blue
        $pageResult = Receive-Job -Job $job
        Remove-Job -Job $job -Force

        # Process successful results and check if we've reached the end of the data.
        if ($null -ne $pageResult) 
        {
            Write-Host "  Retrieved $($pageResult.Count) transactions" -ForegroundColor Green
            $returnResults += $pageResult
    
            # If we get fewer transactions than the batch size, we've reached the end.
            if ($pageResult.Count -lt $BATCH_SIZE) 
            {
                Write-Host "  Less than $($BATCH_SIZE) transactions returned. Assuming end of data for address '$($Address)'." -ForegroundColor Yellow
                $endOfData = $true
                break
            }
        }
    }

    return [PSCustomObject]@{
        EndOfData = $endOfData
        Transactions = $returnResults
        FailedOffsets = $failedOffsets
    }
}

<# -----------------------------------------------------------------
MAIN                                                               |
----------------------------------------------------------------- #>

# First check if the address has any transactions at all to avoid unnecessary processing.
$transactionsCount = Get-TransactionsCountForAddress -Address $Address
if (-not($transactionsCount.Total -gt 0)) 
{ 
    Write-Host "⚠️ No transactions found for address '$($Address)'." -ForegroundColor Yellow
    return $null 
}

# Small optimization, since we know that if .LimitExceeded is false, we have less than 10K transactions (with current API implementation), we can safely manage this in one sweep.
if (-not $transactionsCount.LimitExceeded -and $transactionsCount.Total -le 10000)
{ $ConcurrencyLimit = [math]::Ceiling($transactionsCount.Total / $BATCH_SIZE) }

Write-Host "`n🚀 Starting parallel transaction retrieval mode with $($ConcurrencyLimit) concurrent job(s)..." -ForegroundColor Cyan

$retrievedTransactions = @()    # Stores all successfully retrieved transactions.
$failedOffsets = @()            # Tracks any failed batch retrievals by their offset.
$pendingOffsets = [System.Collections.Generic.Queue[uint]]::new()

$page = 0
$mainPass = $true

$retryCount = 0
$retryPass = $false

while ($true) 
{
    if ($mainPass -eq $true)
    {
        for ($i = 0; $i -lt $ConcurrencyLimit; $i++) 
        {
            $currentOffset = ($page + $i) * $BATCH_SIZE
            $pendingOffsets.Enqueue($currentOffset)
        }

        Write-Host "`n📄 Starting batch at page $($page)..." -ForegroundColor Magenta
        $startResult = Start-TransactionRetrievalJobsByOffsets -Address $Address -ConcurrencyLimit $ConcurrencyLimit -ResolvePreviousOutpoints $ResolvePreviousOutpoints -Fields $Fields -PendingOffsets $pendingOffsets
        
        Write-Host "⌛ Waiting for $($startResult.Jobs.Count) job(s) to complete..." -ForegroundColor Cyan
        $null = $startResult.Jobs | Wait-Job   # Wait for all jobs in this batch to complete before processing their results.
        
        # Process the results from each job.
        $resolveResult = Resolve-TransactionRetrievalResult -RetrievalResult $startResult -Address $Address
        $retrievedTransactions += $resolveResult.Transactions
        $failedOffsets += $resolveResult.FailedOffsets
    
        if ($resolveResult.EndOfData -eq $true) 
        { 
            Write-Host "✔️ Finished main pass. Entering retry mode if needed..." -ForegroundColor Green
            $pendingOffsets.Clear()
            $mainPass = $false
            $retryPass = ($failedOffsets.Count -gt 0) -and ($MaxFailedTries -gt 0)
            continue
        }

        # Advance to the next set of pages based on our concurrency limit.
        $page = $page + $ConcurrencyLimit;
    }
    
    if ($retryPass -eq $true)
    {
        if ($pendingOffsets.Count -eq 0)
        {
            # Ensure we have any failed offsets to work on.
            if ($failedOffsets.Count -eq 0) 
            {
                Write-Host "✅ All failed offsets processed. Exiting retry mode." -ForegroundColor Green
                $retryPass = $false
                continue
            }

            foreach($offset in $failedOffsets) 
            { 
                Write-Host "🔁 Retrying offset $($offset)..." -ForegroundColor DarkCyan
                $pendingOffsets.Enqueue($offset) 
            }

            $failedOffsets.Clear()
            $retryCount++
            Write-Host "🔄 Retry attempt $($retryCount) of $($MaxFailedTries)" -ForegroundColor DarkYellow
        }

        $startResult = Start-TransactionRetrievalJobsByOffsets -Address $Address -ConcurrencyLimit $ConcurrencyLimit -ResolvePreviousOutpoints $ResolvePreviousOutpoints -Fields $Fields -PendingOffsets $pendingOffsets

        Write-Host "⌛ Waiting for $($startResult.Jobs.Count) job(s) to complete..." -ForegroundColor Cyan
        $null = $startResult.Jobs | Wait-Job   # Wait for all jobs in this batch to complete before processing their results.

        # Process the results from each job.
        $resolveResult = Resolve-TransactionRetrievalResult -RetrievalResult $startResult -Address $Address
        $retrievedTransactions += $resolveResult.Transactions
        $failedOffsets += $resolveResult.FailedOffsets

        if (-not($retryCount -lt $MaxFailedTries)) 
        { 
            Write-Host "❌ Max retry attempts reached. Skipping remaining failed offsets." -ForegroundColor Red
            $retryPass = $false
        }
    }

    # Are we done?
    if ($mainPass -eq $false -and $retryPass -eq $false) 
    { 
        Write-Host "`n✅ Transaction retrieval complete!" -ForegroundColor Green
        break 
    }

    # Brief delay to prevent overwhelming the API server with requests.
    # Adjust this value based on the API's rate limiting policies.
    Start-Sleep -Seconds 1
}

<# -----------------------------------------------------------------
OUTPUT                                                             |
----------------------------------------------------------------- #>

return [PSCustomObject]@{
    TransactionsCount = $retrievedTransactions.Count    # Total number of transactions successfully retrieved.
    Transactions = $retrievedTransactions               # The actual transaction data.
    FailedOffsets = $failedOffsets                      # List of offsets that failed (useful for retries).
}