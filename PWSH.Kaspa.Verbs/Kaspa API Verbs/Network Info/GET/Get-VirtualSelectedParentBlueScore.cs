using System.Management.Automation;
using System.Text.Json;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

using LanguageExt;
using static LanguageExt.Prelude;

namespace PWSH.Kaspa.Verbs
{
    /// <summary>
    /// Returns the blue score of virtual selected parent.
    /// Every block in the DAG (aside from the genesis) has a selected parent. 
    /// That selected parent likewise has a selected parent. 
    /// Following this chain of selected parents will eventually bring us to the genesis. 
    /// We call this chain the Selected Parent Chain of a block, or its SPC.
    /// The virtual block is a special, invisible block whose parents are always the blocks in the DAG that do not yet have any children.
    /// Like all blocks, the virtual has a selected parent block.
    /// The Selected Parent Chain of the virtual is plainly called the Virtual Selected Parent Chain, or the VSPC.
    /// </summary>
    [Cmdlet(KaspaVerbNames.Get, "VirtualSelectedParentBlueScore")]
    [OutputType(typeof(ulong))]
    public sealed partial class GetVirtualSelectedParentBlueScore : KaspaPSCmdlet
    {
        private KaspaJob<ulong>? _job;

/* -----------------------------------------------------------------
CONSTRUCTORS                                                       |
----------------------------------------------------------------- */

        public GetVirtualSelectedParentBlueScore()
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
            async Task<Either<ErrorRecord, ulong>> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

            var thisName = this.MyInvocation.MyCommand.Name;
            this._job = new KaspaJob<ulong>(processLogic, thisName);
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
            => "info/virtual-chain-blue-score";

        private async Task<Either<ErrorRecord, ulong>> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
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

                        return Right<ErrorRecord, ulong>(message.RightToList()[0].BlueScore);
                    },
                    Left: err => err
                );
            }
            catch (OperationCanceledException)
            { return new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this); }
            catch (Exception e)
            { return new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this); }
        }
    }
}
