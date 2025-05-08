namespace PWSH.Kaspa.Verbs;

public sealed partial class GetHealthState
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("kaspadServers")]
        public List<KaspadResponseSchema>? KaspadServers { get; set; }

        [JsonPropertyName("database")]
        public DBCheckStatusResponseSchema? Database { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class KaspadResponseSchema
    {
        [JsonPropertyName("kaspadHost")]
        public string? KaspadHost { get; set; }

        [JsonPropertyName("serverVersion")]
        public string? ServerVersion { get; set; }

        [JsonPropertyName("isUtxoIndexed")]
        public bool IsUTXOIndexed { get; set; }

        [JsonPropertyName("isSynced")]
        public bool IsSynced { get; set; }

        [JsonPropertyName("p2pId")]
        public string? P2PID { get; set; }

        [JsonPropertyName("blueScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlueScore { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class DBCheckStatusResponseSchema
    {
        [JsonPropertyName("isSynced")]
        public bool IsSynced { get; set; }

        [JsonPropertyName("blueScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlueScore { get; set; }

        [JsonPropertyName("blueScoreDiff")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlueScoreDiff { get; set; }
    }
}
