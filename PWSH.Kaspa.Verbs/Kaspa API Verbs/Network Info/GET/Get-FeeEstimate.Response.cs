namespace PWSH.Kaspa.Verbs;

public sealed partial class GetFeeEstimate
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("priorityBucket")]
        public FeeEstimateBucketResponseSchema? PriorityBucket { get; set; }

        [JsonPropertyName("normalBuckets")]
        public List<FeeEstimateBucketResponseSchema>? NormalBuckets { get; set; }

        [JsonPropertyName("lowBuckets")]
        public List<FeeEstimateBucketResponseSchema>? LowBuckets { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class FeeEstimateBucketResponseSchema
    {
        [JsonPropertyName("feerate")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Feerate { get; set; }

        [JsonPropertyName("estimatedSeconds")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal EstimatedSeconds { get; set; }
    }
}
