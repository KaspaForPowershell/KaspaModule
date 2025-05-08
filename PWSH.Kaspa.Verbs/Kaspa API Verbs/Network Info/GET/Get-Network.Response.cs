namespace PWSH.Kaspa.Verbs;

public sealed partial class GetNetwork
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("networkName")]
        public string? NetworkName { get; set; }

        [JsonPropertyName("blockCount")]
        public string? BlockCount { get; set; }

        [JsonPropertyName("headerCount")]
        public string? HeaderCount { get; set; }

        [JsonPropertyName("tipHashes")]
        public List<string>? TipHashes { get; set; }

        [JsonPropertyName("difficulty")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Difficulty { get; set; }

        [JsonPropertyName("pastMedianTime")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long PastMedianTime { get; set; }

        [JsonPropertyName("virtualParentHashes")]
        public List<string>? VirtualParentHashes { get; set; }

        [JsonPropertyName("pruningPointHash")]
        public string? PruningPointHash { get; set; }

        [JsonPropertyName("virtualDaaScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong VirtualDaaScore { get; set; }
    }
}
