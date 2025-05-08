namespace PWSH.Kaspa.Verbs;

public sealed partial class GetHalving
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("nextHalvingTimestamp")]
        [JsonConverter(typeof(StringToLongConverter))]
        public long NextHalvingTimestamp { get; set; }

        [JsonPropertyName("nextHalvingDate")]
        public string? NextHalvingDate { get; set; }

        [JsonPropertyName("nextHalvingAmount")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal NextHalvingAmount { get; set; }
    }
}
