namespace PWSH.Kaspa.Verbs;

public sealed partial class GetBlockDAG
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("networkName")]
        public string? NetworkName { get; set; }

        [JsonPropertyName("blockCount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlockCount { get; set; }

        [JsonPropertyName("headerCount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong HeaderCount { get; set; }

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
