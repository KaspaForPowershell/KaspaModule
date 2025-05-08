namespace PWSH.Kaspa.Verbs;

public sealed partial class GetMarketCap
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("marketcap")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal MarketCap { get; set; }
    }
}
