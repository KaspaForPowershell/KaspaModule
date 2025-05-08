namespace PWSH.Kaspa.Verbs;

public sealed partial class SubmitNewTransaction
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("transaction_id")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
