namespace PWSH.Kaspa.Verbs;

[Cmdlet(KaspaVerbNames.Calculate, "TransactionMass")]
[OutputType(typeof(ResponseSchema))]
public sealed partial class CalculateTransactionMass : KaspaPSCmdlet
{
    private KaspaJob<ResponseSchema>? _job;

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
        async Task<Either<ErrorRecord, ResponseSchema>> processLogic(CancellationToken cancellation_token) { return await DoProcessLogicAsync(this._httpClient!, this._deserializerOptions!, cancellation_token); }

        var thisName = this.MyInvocation.MyCommand.Name;
        this._job = new KaspaJob<ResponseSchema>(processLogic, thisName);
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
        => "transactions/mass";

    private async Task<Either<ErrorRecord, ResponseSchema>> DoProcessLogicAsync(HttpClient http_client, JsonSerializerOptions deserializer_options, CancellationToken cancellation_token)
    {
        try
        {
            var requestSchema = Transaction!.ToMassRequestSchema();

            if (requestSchema.SubnetworkID.CompareString(Globals.MINNER_TRANSACTION_SUBNETWORK))
            {
                // It seems that for miner transactions mass is always 0 https://github.com/kaspa-ng/kaspa-rest-server/pull/63/files
                return new ResponseSchema() { ComputeMass = 0, Mass = 0, StorageMass = 0 };
            }
            else
            {
                var response = await http_client.SendRequestAsync(this, Globals.KASPA_API_ADDRESS, BuildQuery(), HttpMethod.Post, requestSchema, TimeoutSeconds, cancellation_token);
                return await response.MatchAsync
                (
                    RightAsync: async ok => await ok.ProcessResponseAsync<ResponseSchema>(deserializer_options, this, TimeoutSeconds, cancellation_token),
                    Left: err => err
                );
            }
        }
        catch (OperationCanceledException)
        { return new ErrorRecord(new OperationCanceledException("Task was canceled."), "TaskCanceled", ErrorCategory.OperationStopped, this); }
        catch (Exception e)
        { return new ErrorRecord(e, "TaskInvalid", ErrorCategory.InvalidOperation, this); }
    }
}
