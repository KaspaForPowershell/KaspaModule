using System.Management.Automation;
using System.Text.Json;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    [Cmdlet(KaspaVerbNames.Calculate, "TransactionMass")]
    [OutputType(typeof(ResponseSchema))]
    public sealed partial class CalculateTransactionMass : KaspaPSCmdlet
    {
        private KaspaJob<ResponseSchema?>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public CalculateTransactionMass()
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
            async Task<(ResponseSchema?, ErrorRecord?)> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

            var thisName = this.MyInvocation.MyCommand.Name;
            this._job = new KaspaJob<ResponseSchema?>(processLogic, thisName);
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
            => "transactions/mass";

        private (RequestSchema?, ErrorRecord?) BuildRequest()
            => Transaction is null
                ? (null, new ErrorRecord(new NullReferenceException("Processed transaction is null."), "NullParameter", ErrorCategory.InvalidOperation, this))
                : (Transaction.ToMassRequestSchema(), null);

        private async Task<(ResponseSchema?, ErrorRecord?)> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var (requestSchema, err) = BuildRequest();
                if (err is not null) return (default, err);
                if (requestSchema is null) return (default, new ErrorRecord(new InvalidOperationException("Failed to initialize request."), "TaskNull", ErrorCategory.InvalidOperation, this));

                if (requestSchema.SubnetworkID.CompareString(Globals.MINNER_TRANSACTION_SUBNETWORK))
                {
                    // It seems that for miner transactions mass is always 0 https://github.com/kaspa-ng/kaspa-rest-server/pull/63/files
                    return (new() { ComputeMass = 0, Mass = 0, StorageMass = 0 }, null);
                }
                else
                {
                    (var request, err) = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Post, requestSchema, TimeoutSeconds, cancellation_token);
                    if (err is not null) return (default, err);
                    if (request is null) return (default, new ErrorRecord(new InvalidOperationException("Received a null response from the API."), "TaskNull", ErrorCategory.InvalidOperation, this));

                    (var response, err) = await request.ProcessResponseAsync<ResponseSchema>(deserializer_options, this, TimeoutSeconds, cancellation_token);
                    if (err is not null) return (default, err);
                    if (response is null) return (default, new ErrorRecord(new InvalidOperationException("The API response was not initialized."), "TaskNull", ErrorCategory.InvalidOperation, this));

                    return (response, null);
                }
            }
            catch (OperationCanceledException)
            { return (default, new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return (default, new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}
