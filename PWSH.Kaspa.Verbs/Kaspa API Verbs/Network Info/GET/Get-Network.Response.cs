using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetNetwork
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("networkName")]
            public string? NetworkName { get; set; }

            [JsonPropertyName("blockCount")]
            public string? BlockCount { get; set; }

            [JsonPropertyName("headerCount")]
            public string? HeaderCount { get; set; }

            [JsonPropertyName("tipHashes")]
            public List<string>? TipHashes { get; set; }

            [JsonPropertyName("difficulty")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal Difficulty { get; set; }

            [JsonPropertyName("pastMedianTime")]
            [JsonConverter(typeof(StringToLongConverter))]
            public long PastMedianTime { get; set; }

            [JsonPropertyName("virtualParentHashes")]
            public List<string>? VirtualParentHashes { get; set; }

            [JsonPropertyName("pruningPointHash")]
            public string? PruningPointHash { get; set; }

            [JsonPropertyName("virtualDaaScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong VirtualDaaScore { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    NetworkName == other.NetworkName &&
                    BlockCount == other.BlockCount &&
                    HeaderCount == other.HeaderCount &&
                    TipHashes.CompareList(other.TipHashes) &&
                    Difficulty == other.Difficulty &&
                    PastMedianTime == other.PastMedianTime &&
                    VirtualParentHashes.CompareList(other.VirtualParentHashes) &&
                    PruningPointHash == other.PruningPointHash &&
                    VirtualDaaScore == other.VirtualDaaScore;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ResponseSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(NetworkName, BlockCount, HeaderCount);
                hash = TipHashes.GenerateHashCode(hash);
                hash = HashCode.Combine(hash, Difficulty, PastMedianTime);
                hash = VirtualParentHashes.GenerateHashCode(hash);
                return HashCode.Combine(hash, PruningPointHash, VirtualDaaScore);
            }

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
    }
}
