namespace PWSH.Kaspa.Verbs;

public sealed partial class GetHashrate
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("hashrate")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Hashrate { get; set; }
    }
}
