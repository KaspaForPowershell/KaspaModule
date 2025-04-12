using System.Management.Automation;
using System.Text.Json;
using System.Web;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Get circulating amount of $KAS token as numerical value.
    /// </summary>
    [Cmdlet(KaspaVerbNames.Get, "CirculatingCoins")]
    [OutputType(typeof(decimal))]
    public sealed partial class GetCirculatingCoins : KaspaPSCmdlet
    {
        private KaspaJob<decimal>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public GetCirculatingCoins()
        {
            this._httpClient = KaspaModuleInitializer.Instance?.HttpClient;

            if (this._httpClient is null)
                ThrowTerminatingError(new ErrorRecord(new NullReferenceException(), "NullHttpClient", ErrorCategory.InvalidOperation, this));
        }

/* -----------------------------------------------------------------
PROCESS                                                            |
----------------------------------------------------------------- */

        protected override void BeginProcessing()
        {
            async Task<(decimal, ErrorRecord?)> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

            var thisName = this.MyInvocation.MyCommand.Name;
            this._job = new KaspaJob<decimal>(processLogic, thisName);
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
            queryParams["in_billion"] = InBillion.IsPresent.ToString().ToLower();

            return "info/coinsupply/circulating?" + queryParams.ToString();
        }

        private async Task<(decimal, ErrorRecord?)> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var (request, err) = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Get, null, TimeoutSeconds, cancellation_token);
                if (err is not null) return (default, err);
                if (request is null) return (default, new ErrorRecord(new InvalidOperationException("Received a null response from the API."), "TaskNull", ErrorCategory.InvalidOperation, this));

                (var response, err) = await request.ProcessResponseRAWAsync(this, TimeoutSeconds, cancellation_token);
                if (err is not null) return (default, err);
                if (response is null) return (default, new ErrorRecord(new InvalidOperationException("The API response was not initialized."), "TaskNull", ErrorCategory.InvalidOperation, this));

                if (!decimal.TryParse(response, out var result))
                    return (default, new ErrorRecord(new ParseException("JSON parse failed."), "ParseFailed", ErrorCategory.ParserError, this));

                return (result, null);
            }
            catch (OperationCanceledException)
            { return (default, new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return (default, new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}
