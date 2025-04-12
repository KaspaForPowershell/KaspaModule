using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetMaxHashrate
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("hashrate")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal Hashrate { get; set; }

            [JsonPropertyName("blockheader")]
            public BlockHeaderResponseSchema? BlockHeader { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Hashrate == other.Hashrate &&
                    BlockHeader == other.BlockHeader;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Hashrate, BlockHeader);

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

        private sealed class BlockHeaderResponseSchema : IEquatable<BlockHeaderResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("hash")]
            public string? Hash { get; set; }

            [JsonPropertyName("timestamp")]
            public string? Timestamp { get; set; }

            [JsonPropertyName("difficulty")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal Difficulty { get; set; }

            [JsonPropertyName("daaScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong DaaScore { get; set; }

            [JsonPropertyName("blueScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlueScore { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockHeaderResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Hash.CompareString(other.Hash) &&
                    Timestamp == other.Timestamp &&
                    Difficulty == other.Difficulty &&
                    DaaScore == other.DaaScore &&
                    BlueScore == other.BlueScore;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockHeaderResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Hash, Timestamp, Difficulty, DaaScore, BlueScore);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockHeaderResponseSchema? left, BlockHeaderResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(BlockHeaderResponseSchema? left, BlockHeaderResponseSchema? right)
                => !(left == right);
        }
    }
}
