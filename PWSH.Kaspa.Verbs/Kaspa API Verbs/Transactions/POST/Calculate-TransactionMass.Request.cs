namespace PWSH.Kaspa.Verbs;

public sealed partial class CalculateTransactionMass 
{
    [GenerateRequestSchemaBoilerplate]
    public sealed partial class RequestSchema
    {
        [JsonPropertyName("version")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Version { get; set; }

        [JsonPropertyName("inputs")]
        public List<TransactionInputRequestSchema>? Inputs { get; set; }

        [JsonPropertyName("outputs")]
        public List<TransactionOutputRequestSchema>? Outputs { get; set; }

        [JsonPropertyName("lockTime")]
        public ulong LockTime { get; set; }

        [JsonPropertyName("subnetworkId")]
        public string? SubnetworkID { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class TransactionOutputRequestSchema
    {
        [JsonPropertyName("amount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Amount { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public ScriptPublicKeyRequestSchema? ScriptPublicKey { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class ScriptPublicKeyRequestSchema
    {
        [JsonPropertyName("version")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Version { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public string? ScriptPublicKey { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class TransactionInputRequestSchema
    {
        [JsonPropertyName("previousOutpoint")]
        public PreviousOutpointRequestSchema? PreviousOutpoint { get; set; }

        [JsonPropertyName("signatureScript")]
        public string? SignatureScript { get; set; }

        [JsonPropertyName("sequence")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Sequence { get; set; }

        [JsonPropertyName("sigOpCount")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint SigOpCount { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    public sealed partial class PreviousOutpointRequestSchema
    {
        [JsonPropertyName("transactionId")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("index")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Index { get; set; }
    }
}
