namespace PWSH.Kaspa.Verbs;

public sealed partial class GetTransactionsCountForAddress
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("total")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Total { get; set; }

        [JsonPropertyName("limit_exceeded")]
        public bool LimitExceeded { get; set; }
    }
}
