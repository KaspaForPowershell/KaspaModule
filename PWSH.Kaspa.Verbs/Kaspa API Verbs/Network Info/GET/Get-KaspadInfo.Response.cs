namespace PWSH.Kaspa.Verbs;

public sealed partial class GetKaspadInfo
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("mempoolSize")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong MempoolSize { get; set; }

        [JsonPropertyName("serverVersion")]
        public string? ServerVersion { get; set; }

        [JsonPropertyName("isUtxoIndexed")]
        public bool IsUTXOIndexed { get; set; }

        [JsonPropertyName("isSynced")]
        public bool IsSynced { get; set; }

        [JsonPropertyName("p2pIdHashed")]
        public string? P2PIDHashed { get; set; }
    }
}
