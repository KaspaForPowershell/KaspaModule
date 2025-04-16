using System.Management.Automation;
using System.Text.Json;
using System.Web;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

using LanguageExt;
using static LanguageExt.Prelude;

namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Get all transactions for a given address from database. And then get their related full transaction data.
    /// </summary>
    [Cmdlet(KaspaVerbNames.Get, "FullTransactionsForAddress")]
    [OutputType(typeof(List<ResponseSchema>))]
    public sealed partial class GetFullTransactionsForAddress : KaspaPSCmdlet
    {
        private KaspaJob<List<ResponseSchema>>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public GetFullTransactionsForAddress()
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
                if (result.IsLeft)
                {
                    WriteError(result.LeftToList()[0]);
                    return;
                }

                var response = result.RightToList()[0];
                WriteObject(response);
            }
        }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

        protected override string BuildQuery()
        {
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["limit"] = Limit.ToString();
            queryParams["offset"] = Offset.ToString();
            queryParams["resolve_previous_outpoints"] = ResolvePreviousOutpoints.ToString().ToLower();
            if (!string.IsNullOrEmpty(Fields)) queryParams["fields"] = Fields;

            return $"addresses/{Address}/full-transactions?" + queryParams.ToString();
        }

        private async Task<Either<ErrorRecord, List<ResponseSchema>>> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var result = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Get, null, TimeoutSeconds, cancellation_token);
                if (result.IsLeft)
                    return result.LeftToList()[0];

                var response = result.RightToList()[0];
                var message= await response.ProcessResponseAsync<List<ResponseSchema>>(deserializer_options, this, TimeoutSeconds, cancellation_token);
                if (message.IsLeft)
                    return message.LeftToList()[0];

                return Right<ErrorRecord, List<ResponseSchema>>(message.RightToList()[0]);
            }
            catch (OperationCanceledException)
            { return Left<ErrorRecord, List<ResponseSchema>>(new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return Left<ErrorRecord, List<ResponseSchema>>(new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}
