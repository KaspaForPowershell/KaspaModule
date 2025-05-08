namespace PWSH.Kaspa.Verbs;

public sealed partial class GetUTXOsForAddresses
{
    [GenerateRequestSchemaBoilerplate]
    private sealed partial class RequestSchema
    {
        [JsonPropertyName("addresses")]
        public List<string>? Addresses { get; set; }
    }
}
