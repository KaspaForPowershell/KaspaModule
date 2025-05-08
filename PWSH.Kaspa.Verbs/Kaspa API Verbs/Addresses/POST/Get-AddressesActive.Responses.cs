namespace PWSH.Kaspa.Verbs;

public sealed partial class GetAddressesActive
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}
