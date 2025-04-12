using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetHealthState
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("kaspadServers")]
            public List<KaspadResponseSchema>? KaspadServers { get; set; }

            [JsonPropertyName("database")]
            public DBCheckStatusResponseSchema? Database { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    KaspadServers.CompareList(other.KaspadServers) &&
                    Database == other.Database;
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
                var hash = KaspadServers.GenerateHashCode(0);
                return HashCode.Combine(hash, Database);
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

        private sealed class KaspadResponseSchema : IEquatable<KaspadResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("kaspadHost")]
            public string? KaspadHost { get; set; }

            [JsonPropertyName("serverVersion")]
            public string? ServerVersion { get; set; }

            [JsonPropertyName("isUtxoIndexed")]
            public bool IsUTXOIndexed { get; set; }

            [JsonPropertyName("isSynced")]
            public bool IsSynced { get; set; }

            [JsonPropertyName("p2pId")]
            public string? P2PID { get; set; }

            [JsonPropertyName("blueScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlueScore { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(KaspadResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    KaspadHost.CompareString(other.KaspadHost) &&
                    ServerVersion.CompareString(other.ServerVersion) &&
                    IsUTXOIndexed == other.IsUTXOIndexed &&
                    IsSynced == other.IsSynced &&
                    P2PID.CompareString(other.P2PID) &&
                    BlueScore == other.BlueScore;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as KaspadResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(KaspadHost, ServerVersion, IsUTXOIndexed, IsSynced, P2PID, BlueScore);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(KaspadResponseSchema? left, KaspadResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(KaspadResponseSchema? left, KaspadResponseSchema? right)
                => !(left == right);
        }

        private sealed class DBCheckStatusResponseSchema : IEquatable<DBCheckStatusResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("isSynced")]
            public bool IsSynced { get; set; }

            [JsonPropertyName("blueScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlueScore { get; set; }

            [JsonPropertyName("blueScoreDiff")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlueScoreDiff { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(DBCheckStatusResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    IsSynced == other.IsSynced &&
                    BlueScore == other.BlueScore &&
                    BlueScoreDiff == other.BlueScoreDiff;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as DBCheckStatusResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(IsSynced, BlueScore, BlueScoreDiff);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(DBCheckStatusResponseSchema? left, DBCheckStatusResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(DBCheckStatusResponseSchema? left, DBCheckStatusResponseSchema? right)
                => !(left == right);
        }
    }
}
