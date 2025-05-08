namespace PWSH.Kaspa.Verbs;

public sealed partial class GetTransaction
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("subnetwork_id")]
        public string? SubnetworkID { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        [JsonPropertyName("mass")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Mass { get; set; }

        [JsonPropertyName("payload")]
        public string? Payload { get; set; }

        [JsonPropertyName("block_hash")]
        public List<string>? BlockHash { get; set; }

        [JsonPropertyName("block_time")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlockTime { get; set; }

        [JsonPropertyName("is_accepted")]
        public bool IsAccepted { get; set; }

        [JsonPropertyName("accepting_block_hash")]
        public string? AcceptingBlockHash { get; set; }

        [JsonPropertyName("accepting_block_blue_score")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong AcceptingBlockBlueScore { get; set; }

        [JsonPropertyName("accepting_block_time")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong AcceptingBlockTime { get; set; }

        [JsonPropertyName("inputs")]
        public List<TransactionInputResponseSchema>? Inputs { get; set; }

        [JsonPropertyName("outputs")]
        public List<TransactionOutputResponseSchema>? Outputs { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class TransactionInputResponseSchema
    {
        [JsonPropertyName("transaction_id")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("index")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Index { get; set; }

        [JsonPropertyName("previous_outpoint_hash")]
        public string? PreviousOutpointHash { get; set; }

        [JsonPropertyName("previous_outpoint_index")]
        public string? PreviousOutpointIndex { get; set; }

        [JsonPropertyName("previous_outpoint_resolved")]
        public TransactionOutputResponseSchema? PreviousOutpointResolved { get; set; }

        [JsonPropertyName("previous_outpoint_address")]
        public string? PreviousOutpointAddress { get; set; }

        [JsonPropertyName("previous_outpoint_amount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong PreviousOutpointAmount { get; set; }

        [JsonPropertyName("signature_script")]
        public string? SignatureScript { get; set; }

        [JsonPropertyName("sig_op_count")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint SigOpCount { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class TransactionOutputResponseSchema
    {
        [JsonPropertyName("transaction_id")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("index")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Index { get; set; }

        [JsonPropertyName("amount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Amount { get; set; }

        [JsonPropertyName("script_public_key")]
        public string? ScriptPublicKey { get; set; }

        [JsonPropertyName("script_public_key_address")]
        public string? ScriptPublicKeyAddress { get; set; }

        [JsonPropertyName("script_public_key_type")]
        public string? ScriptPublicKeyType { get; set; }

        [JsonPropertyName("accepting_block_hash")]
        public string? AcceptingBlockHash { get; set; }
    }
}
