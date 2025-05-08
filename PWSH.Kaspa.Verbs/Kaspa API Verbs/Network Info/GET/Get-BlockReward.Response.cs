namespace PWSH.Kaspa.Verbs;

public sealed partial class GetBlockReward
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("blockreward")]
        public decimal BlockReward { get; set; }
    }
}
