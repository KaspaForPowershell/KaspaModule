using System.Management.Automation;
using System.Text.Json;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

using LanguageExt;
using static LanguageExt.Prelude;

namespace PWSH.Kaspa.Verbs
{
    [Cmdlet(KaspaVerbNames.Get, "Price")]
    [OutputType(typeof(ResponseSchema))]
    public sealed partial class GetPrice : KaspaPSCmdlet
    {
        private KaspaJob<decimal>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public GetPrice()
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
            async Task<Either<ErrorRecord, decimal>> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

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

        protected override string BuildQuery()
            => "info/price";

        private async Task<Either<ErrorRecord, decimal>> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var response = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Get, null, TimeoutSeconds, cancellation_token);
                return await response.MatchAsync
                (
                    RightAsync: async ok =>
                    {
                        var message = await ok.ProcessResponseAsync<ResponseSchema>(deserializer_options, this, TimeoutSeconds, cancellation_token);
                        if (message.IsLeft)
                            return message.LeftToList()[0];

                        return Right<ErrorRecord, decimal>(message.RightToList()[0].Price);
                    },
                    Left: err => Left<ErrorRecord, decimal>(err)
                );
            }
            catch (OperationCanceledException)
            { return Left<ErrorRecord, decimal>(new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return Left<ErrorRecord, decimal>(new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}