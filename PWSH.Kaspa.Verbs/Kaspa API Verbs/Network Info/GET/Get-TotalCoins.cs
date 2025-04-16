using System.Management.Automation;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

using LanguageExt;
using static LanguageExt.Prelude;
using static PWSH.Kaspa.Verbs.GetVirtualSelectedParentBlueScore;

namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Get total amount of $KAS token as numerical value.
    /// </summary>
    [Cmdlet(KaspaVerbNames.Get, "TotalCoins")]
    [OutputType(typeof(decimal))]
    public sealed partial class GetTotalCoins : KaspaPSCmdlet
    {
        private KaspaJob<decimal>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public GetTotalCoins()
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
            async Task<Either<ErrorRecord, decimal>> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, cancellation_token); }

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
                var result = DoProcessLogicAsync(this._httpClient!, stoppingToken).GetAwaiter().GetResult();
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
            => "info/coinsupply/total";

        private async Task<Either<ErrorRecord, decimal>> DoProcessLogicAsync(HttpClient http_client, CancellationToken cancellation_token)
        {
            try
            {
                var result = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Get, null, TimeoutSeconds, cancellation_token);
                if (result.IsLeft)
                    return result.LeftToList()[0];

                var response = result.RightToList()[0];
                var message = await response.ProcessResponseRAWAsync(this, TimeoutSeconds, cancellation_token);
                if (message.IsLeft)
                    return message.LeftToList()[0];

                if (!decimal.TryParse(message.RightToList()[0], out var parsed))
                    return Left<ErrorRecord, decimal>(new ErrorRecord(new ParseException("JSON parse failed."), "ParseFailed", ErrorCategory.ParserError, this));

                return Right<ErrorRecord, decimal>(parsed);
            }
            catch (OperationCanceledException)
            { return Left<ErrorRecord, decimal>(new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return Left<ErrorRecord, decimal>(new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}
