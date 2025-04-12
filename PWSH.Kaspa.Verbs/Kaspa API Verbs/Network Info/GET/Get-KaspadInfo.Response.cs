using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetKaspadInfo
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("mempoolSize")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong MempoolSize { get; set; }

            [JsonPropertyName("serverVersion")]
            public string? ServerVersion { get; set; }

            [JsonPropertyName("isUtxoIndexed")]
            public bool IsUTXOIndexed { get; set; }

            [JsonPropertyName("isSynced")]
            public bool IsSynced { get; set; }

            [JsonPropertyName("p2pIdHashed")]
            public string? P2PIDHashed { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    MempoolSize == other.MempoolSize &&
                    ServerVersion.CompareString(other.ServerVersion) &&
                    IsUTXOIndexed == other.IsUTXOIndexed &&
                    IsSynced == other.IsSynced &&
                    P2PIDHashed.CompareString(other.P2PIDHashed);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(MempoolSize, ServerVersion, IsUTXOIndexed, IsSynced, P2PIDHashed);

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
