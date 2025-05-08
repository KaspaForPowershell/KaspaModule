namespace PWSH.Kaspa.Verbs;

public sealed partial class GetBalancesFromAddresses
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("balance")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Balance { get; set; }
    }
}
