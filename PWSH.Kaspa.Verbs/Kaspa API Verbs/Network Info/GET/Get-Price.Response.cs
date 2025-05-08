namespace PWSH.Kaspa.Verbs;

public sealed partial class GetPrice
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("price")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Price { get; set; }
    }
}