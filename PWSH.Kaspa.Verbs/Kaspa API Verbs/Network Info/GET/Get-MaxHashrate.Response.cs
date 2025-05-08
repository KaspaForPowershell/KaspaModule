namespace PWSH.Kaspa.Verbs;

public sealed partial class GetMaxHashrate
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("hashrate")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Hashrate { get; set; }

        [JsonPropertyName("blockheader")]
        public BlockHeaderResponseSchema? BlockHeader { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockHeaderResponseSchema
    {
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }

        [JsonPropertyName("timestamp")]
        public string? Timestamp { get; set; }

        [JsonPropertyName("difficulty")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Difficulty { get; set; }

        [JsonPropertyName("daaScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong DaaScore { get; set; }

        [JsonPropertyName("blueScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlueScore { get; set; }
    }
}
