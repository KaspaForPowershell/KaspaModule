using System.Management.Automation;
using System.Text.Json;
using System.Web;
using PWSH.Kaspa.Constants;
using PWSH.Kaspa.Base;


namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Get details for a given transaction ID.
    /// </summary>
    [CmdletBinding(DefaultParameterSetName = SearchForTransactionsParameterSetName.TRANSACTION_ID)]
    [Cmdlet(KaspaVerbNames.Search, "ForTransactions")]
    [OutputType(typeof(List<ResponseSchema>))]
    public sealed partial class SearchForTransactions : KaspaPSCmdlet
    {
        private KaspaJob<List<ResponseSchema>?>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public SearchForTransactions()
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
            async Task<(List<ResponseSchema>?, ErrorRecord?)> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

            var thisName = this.MyInvocation.MyCommand.Name;
            this._job = new KaspaJob<List<ResponseSchema>?>(processLogic, thisName);
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
                var (responce, err) = DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, stoppingToken).GetAwaiter().GetResult();
                if (err is not null)
                {
                    WriteError(err);
                    return;
                }

                WriteObject(responce);
            }
        }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

        protected override string BuildQuery()
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["resolve_previous_outpoints"] = ResolvePreviousOutpoints.ToString().ToLower();
            if (!string.IsNullOrEmpty(Fields)) queryParams["fields"] = Fields.ToString();

            return $"transactions/search?" + queryParams.ToString();
        }

        private async Task<(List<ResponseSchema>?, ErrorRecord?)> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var requestSchema = this.ParameterSetName switch
                {
                    SearchForTransactionsParameterSetName.BLUE_SCORE => new RequestSchema() { AcceptingBlueScores = new AcceptingBlueScoreRequestSchema() { Gte = Gte, Lt = Lt } },
                    _ => new RequestSchema() { TransactionIDs = TransactionIDs }
                };

                var (request, err) = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Post, requestSchema, TimeoutSeconds, cancellation_token);
                if (err is not null) return (default, err);
                if (request is null) return (default, new ErrorRecord(new InvalidOperationException("Received a null response from the API."), "TaskNull", ErrorCategory.InvalidOperation, this));

                (var response, err) = await request.ProcessResponseAsync<List<ResponseSchema>>(deserializer_options, this, TimeoutSeconds, cancellation_token);
                if (err is not null) return (default, err);
                if (response is null) return (default, new ErrorRecord(new InvalidOperationException("The API response was not initialized."), "TaskNull", ErrorCategory.InvalidOperation, this));

                return (response, null);
            }
            catch (OperationCanceledException)
            { return (default, new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return (default, new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}
