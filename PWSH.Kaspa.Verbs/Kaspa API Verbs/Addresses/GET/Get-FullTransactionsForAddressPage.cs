namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Get all transactions for a given address from database. 
    /// And then get their related full transaction data.
    /// </summary>
    [CmdletBinding(DefaultParameterSetName = GetFullTransactionsForAddressPageParameterSetName.BEFORE_TIMESTAMP)]
    [Cmdlet(KaspaVerbNames.Get, "FullTransactionsForAddressPage")]
    [OutputType(typeof(List<ResponseSchema>))]
    public sealed partial class GetFullTransactionsForAddressPage : KaspaPSCmdlet
    {
        private KaspaJob<List<ResponseSchema>>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public GetFullTransactionsForAddressPage() 
        {
            this._httpClient = KaspaModuleInitializer.Instance?.HttpClient;
            this._deserializerOptions = KaspaModuleInitializer.Instance?.ResponseDeserializer;

            if (this._httpClient is null || this._deserializerOptions is null)
                ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "NullHttpClient", ErrorCategory.InvalidOperation, this));
        }

/* -----------------------------------------------------------------
PROCESS                                                            |
----------------------------------------------------------------- */

        protected override void BeginProcessing()
        {
            async Task<Either<ErrorRecord, List<ResponseSchema>>> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

            var thisName = this.MyInvocation.MyCommand.Name;
            this._job = new KaspaJob<List<ResponseSchema>>(processLogic, thisName);
        }

        protected override void ProcessRecord()
        {
            var stoppingToken = this.CreateStoppingToken();

            if (AsJob.IsPresent)
            {
                if (this._job is null)
                {
                    WriteError(new ErrorRecord(new NullReferenceException("The job was not initialized."), "JobExecutionFailure", ErrorCategory.InvalidOperation, this));
                    return;
                }

                JobRepository.Add(this._job);
                WriteObject(this._job);

                var jobTask = Task.Run(async () => await this._job.ProcessJob(stoppingToken));
                jobTask.ContinueWith(t =>
                {
                    if (t.Exception is not null) WriteError(new ErrorRecord(t.Exception, "JobExecutionFailure", ErrorCategory.OperationStopped, this));
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
            else
            {
                var result = DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, stoppingToken).GetAwaiter().GetResult();
                result.Match
                (
                    Right: ok => WriteObject(ok),
                    Left: err => WriteError(err)
                );
            }
        }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

        private string BuildQuery(string? next_page)
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["limit"] = Limit.ToString();

            var directionString = this.ParameterSetName == GetFullTransactionsForAddressPageParameterSetName.BEFORE_TIMESTAMP
                ? "before"
                : "after";

            if (next_page is not null) queryParams[directionString] = next_page;

            queryParams["resolve_previous_outpoints"] = ResolvePreviousOutpoints.ToString().ToLower();
            if (!string.IsNullOrEmpty(Fields)) queryParams["fields"] = Fields;

            return $"addresses/{Address}/full-transactions-page?" + queryParams.ToString();
        }

        private async Task<Either<ErrorRecord, List<ResponseSchema>>> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var output = new List<ResponseSchema>();
                string? previousPage = null;
                string? nextPage = $"{Timestamp}";
                var hasMorePages = true;
                var currPage = 0u;

                while (hasMorePages)
                {
                    var result = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(nextPage), HttpMethod.Get, null, TimeoutSeconds, cancellation_token);
                    if (result.IsLeft)
                        return result.LeftToList()[0];

                    var response = result.RightToList()[0];
                    var message = await response.ProcessResponseAsync<List<ResponseSchema>>(deserializer_options, this, TimeoutSeconds, cancellation_token);
                    if (message.IsLeft)
                        return message.LeftToList()[0];

                    if (cancellation_token.IsCancellationRequested) 
                        return Left<ErrorRecord, List<ResponseSchema>>(new ErrorRecord(new OperationCanceledException(), "TaskCanceled", ErrorCategory.OperationStopped, this));

                    output.AddRange(message.RightToList()[0]);

                    // Process headers.
                    var headerString = this.ParameterSetName == GetFullTransactionsForAddressPageParameterSetName.BEFORE_TIMESTAMP
                        ? "X-Next-Page-Before"
                        : "X-Next-Page-After";

                    nextPage = response.Headers.TryGetValues(headerString, out var beforeValues)
                     ? beforeValues.FirstOrDefault()
                     : null;

                    hasMorePages = (nextPage is not null) && (nextPage != previousPage);
                    if (hasMorePages)
                    {
                        currPage++;
                        previousPage = nextPage;

                        // Delay to prevent overwhelming the server.
                        await Task.Delay(TimeSpan.FromMilliseconds(PagingDelayMilliseconds), cancellation_token);
                    }

                    // TODO: This is only temporary...
                    //Console.WriteLine($"Page: {currPage}, Total transactions: {output.Count}");
                }

                return Right<ErrorRecord, List<ResponseSchema>>([.. output.OrderBy(tx => tx.BlockTime)]);
            }
            catch (Exception e)
            { return new ErrorRecord(e, "UnhandledException", ErrorCategory.NotSpecified, this); }
        }
    }
}
