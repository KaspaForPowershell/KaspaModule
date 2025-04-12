using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetFeeEstimate
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("priorityBucket")]
            public FeeEstimateBucketResponseSchema? PriorityBucket { get; set; }

            [JsonPropertyName("normalBuckets")]
            public List<FeeEstimateBucketResponseSchema>? NormalBuckets { get; set; }

            [JsonPropertyName("lowBuckets")]
            public List<FeeEstimateBucketResponseSchema>? LowBuckets { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    PriorityBucket == other.PriorityBucket &&
                    NormalBuckets.CompareList(other.NormalBuckets) &&
                    LowBuckets.CompareList(other.LowBuckets);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(PriorityBucket, NormalBuckets, LowBuckets);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(ResponseSchema? left, ResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(ResponseSchema? left, ResponseSchema? right)
                => !(left == right);
        }

        private sealed class FeeEstimateBucketResponseSchema : IEquatable<FeeEstimateBucketResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("feerate")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal Feerate { get; set; }

            [JsonPropertyName("estimatedSeconds")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal EstimatedSeconds { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(FeeEstimateBucketResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Feerate == other.Feerate &&
                    EstimatedSeconds == other.EstimatedSeconds;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as FeeEstimateBucketResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Feerate, EstimatedSeconds);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(FeeEstimateBucketResponseSchema? left, FeeEstimateBucketResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(FeeEstimateBucketResponseSchema? left, FeeEstimateBucketResponseSchema? right)
                => !(left == right);
        }
    }
}
