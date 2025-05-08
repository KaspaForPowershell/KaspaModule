namespace PWSH.Kaspa.Verbs;

public sealed partial class CalculateTransactionMass 
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("mass")]
        public int Mass { get; set; }

        [JsonPropertyName("storage_mass")]
        public int StorageMass { get; set; }

        [JsonPropertyName("compute_mass")]
        public int ComputeMass { get; set; }
    }
}
