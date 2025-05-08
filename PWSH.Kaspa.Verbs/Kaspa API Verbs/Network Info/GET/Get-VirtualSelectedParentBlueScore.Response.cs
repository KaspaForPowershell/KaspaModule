namespace PWSH.Kaspa.Verbs;

public sealed partial class GetVirtualSelectedParentBlueScore
{
    [GenerateResponseSchemaBoilerplate]
    public sealed partial class ResponseSchema
    {
        [JsonPropertyName("blueScore")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong BlueScore { get; set; }
    }
}
