namespace PWSH.Kaspa.Verbs;

public sealed partial class GetUTXOsForAddress
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("outpoint")]
        public OutpointResponseSchema? Outpoint { get; set; }

        [JsonPropertyName("utxoEntry")]
        public UTXOEntryResponseSchema? UTXOEntry { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class OutpointResponseSchema
    {
        [JsonPropertyName("transactionId")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("index")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Index { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class UTXOEntryResponseSchema
    {
        [JsonPropertyName("amount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Amount { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public ScriptPublicKeyModelResponseSchema? ScriptPublicKey { get; set; }

        /// <summary>
        /// The DAA Score is related to the number of honest blocks ever added to the DAG. 
        /// Since blocks are created at a rate of one per second, the score is a metric of the elapsed time since network launch.
        /// </summary>
        [JsonPropertyName("blockDaaScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlockDaaScore { get; set; }

        [JsonPropertyName("isCoinbase")]
        public bool IsCoinbase { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ScriptPublicKeyModelResponseSchema
    {
        [JsonPropertyName("scriptPublicKey")]
        public string? ScriptPublicKey { get; set; }
    }
}
