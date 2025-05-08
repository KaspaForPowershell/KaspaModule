namespace PWSH.Kaspa.Verbs;

public sealed partial class SubmitNewTransaction
{
    [GenerateRequestSchemaBoilerplate]
    private sealed partial class RequestSchema
    {
        [JsonPropertyName("transaction")]
        public TransactionRequestSchema? Transaction { get; set; }

        [JsonPropertyName("allowOrphan")]
        public bool AllowOrphan { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class TransactionRequestSchema
    {
        [JsonPropertyName("version")]
        public uint Version { get; set; }

        [JsonPropertyName("inputs")]
        public List<TransactionInputRequestSchema>? Inputs { get; set; }

        [JsonPropertyName("outputs")]
        public List<TransactionOutputRequestSchema>? Outputs { get; set; }

        [JsonPropertyName("lockTime")]
        public int LockTime { get; set; }

        [JsonPropertyName("subnetworkId")]
        public string? SubnetworkID { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class TransactionInputRequestSchema
    {
        [JsonPropertyName("previousOutpoint")]
        public OutpointRequestSchema? PreviousOutpoint { get; set; }

        [JsonPropertyName("signatureScript")]
        public string? SignatureScript { get; set; }

        [JsonPropertyName("sequence")]
        public int Sequence { get; set; }

        [JsonPropertyName("sigOpCount")]
        public int SigOpCount { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class OutpointRequestSchema
    {
        [JsonPropertyName("transactionId")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class TransactionOutputRequestSchema
    {
        [JsonPropertyName("amount")]
        public ulong Amount { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public ScriptPublicKeyRequestSchema? ScriptPublicKey { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class ScriptPublicKeyRequestSchema
    {
        [JsonPropertyName("version")]
        public uint Version { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public string? ScriptPublicKey { get; set; }
    }
}
