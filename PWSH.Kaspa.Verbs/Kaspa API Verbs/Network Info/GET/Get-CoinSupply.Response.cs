namespace PWSH.Kaspa.Verbs;

public sealed partial class GetCoinSupply
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("circulatingSupply")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong CirculatingSupply { get; set; }

        [JsonPropertyName("maxSupply")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong MaxSupply { get; set; }
    }
}
