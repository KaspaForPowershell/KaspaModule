using System.Management.Automation;
using System.Text.Json;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Constants;

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
            async Task<(ulong, ErrorRecord?)> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

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
            => "info/virtual-chain-blue-score";

        private async Task<(ulong, ErrorRecord?)> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
        {
            try
            {
                var (request, err) = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Get, null, TimeoutSeconds, cancellation_token);
                if (err is not null) return (default, err);
                if (request is null) return (default, new ErrorRecord(new InvalidOperationException("Received a null response from the API."), "TaskNull", ErrorCategory.InvalidOperation, this));

                (var response, err) = await request.ProcessResponseAsync<ResponseSchema>(deserializer_options, this, TimeoutSeconds, cancellation_token);
                if (err is not null) return (default, err);
                if (response is null) return (default, new ErrorRecord(new InvalidOperationException("The API response was not initialized."), "TaskNull", ErrorCategory.InvalidOperation, this));
                
                return (response.BlueScore, null);
            }
            catch (OperationCanceledException)
            { return (default, new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this)); }
            catch (Exception e)
            { return (default, new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this)); }
        }
    }
}
