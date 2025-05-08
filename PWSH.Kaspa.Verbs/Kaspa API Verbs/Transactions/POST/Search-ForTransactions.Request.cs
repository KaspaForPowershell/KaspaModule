namespace PWSH.Kaspa.Verbs;

public sealed partial class SearchForTransactions
{
    [GenerateRequestSchemaBoilerplate]
    private sealed partial class RequestSchema
    {
        [JsonPropertyName("transactionIds")]
        public List<string>? TransactionIDs { get; set; }

        [JsonPropertyName("acceptingBlueScores")]
        public AcceptingBlueScoreRequestSchema? AcceptingBlueScores { get; set; }
    }

    [GenerateRequestSchemaBoilerplate]
    private sealed partial class AcceptingBlueScoreRequestSchema
    {
        [JsonPropertyName("gte")]
        public ulong Gte { get; set; } = 0;

        [JsonPropertyName("lt")]
        public ulong Lt { get; set; } = 0;
    }
}
