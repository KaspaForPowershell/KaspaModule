namespace PWSH.Kaspa.Verbs;

public sealed partial class GetNameForAddress
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }
}
