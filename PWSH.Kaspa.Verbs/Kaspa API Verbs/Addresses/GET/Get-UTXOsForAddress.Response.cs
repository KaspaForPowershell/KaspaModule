using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetUTXOsForAddress
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("address")]
            public string? Address { get; set; }

            [JsonPropertyName("outpoint")]
            public OutpointResponseSchema? Outpoint { get; set; }

            [JsonPropertyName("utxoEntry")]
            public UTXOEntryResponseSchema? UTXOEntry { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Address.CompareString(other.Address) &&
                    Outpoint == other.Outpoint &&
                    UTXOEntry == other.UTXOEntry;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Address, Outpoint, UTXOEntry);

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

        private sealed class OutpointResponseSchema : IEquatable<OutpointResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("transactionId")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("index")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Index { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(OutpointResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    TransactionID.CompareString(other.TransactionID) &&
                    Index == other.Index;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as OutpointResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(TransactionID, Index);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(OutpointResponseSchema? left, OutpointResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(OutpointResponseSchema? left, OutpointResponseSchema? right)
                => !(left == right);
        }

        private sealed class UTXOEntryResponseSchema : IEquatable<UTXOEntryResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("amount")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Amount { get; set; }

            [JsonPropertyName("scriptPublicKey")]
            public ScriptPublicKeyModelResponseSchema? ScriptPublicKey { get; set; }

            /// <summary>
            /// The DAA Score is related to the number of honest blocks ever added to the DAG. 
            /// Since blocks are created at a rate of one per second, the score is a metric of the elapsed time since network launch.
            /// </summary>
            [JsonPropertyName("blockDaaScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlockDaaScore { get; set; }

            [JsonPropertyName("isCoinbase")]
            public bool IsCoinbase { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(UTXOEntryResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Amount == other.Amount &&
                    ScriptPublicKey == other.ScriptPublicKey &&
                    BlockDaaScore == other.BlockDaaScore &&
                    IsCoinbase == other.IsCoinbase;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as UTXOEntryResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Amount, ScriptPublicKey, BlockDaaScore, IsCoinbase);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(UTXOEntryResponseSchema? left, UTXOEntryResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(UTXOEntryResponseSchema? left, UTXOEntryResponseSchema? right)
                => !(left == right);
        }

        private sealed class ScriptPublicKeyModelResponseSchema : IEquatable<ScriptPublicKeyModelResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("scriptPublicKey")]
            public string? ScriptPublicKey { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ScriptPublicKeyModelResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return ScriptPublicKey.CompareString(other.ScriptPublicKey);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ScriptPublicKeyModelResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(ScriptPublicKey);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(ScriptPublicKeyModelResponseSchema? left, ScriptPublicKeyModelResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(ScriptPublicKeyModelResponseSchema? left, ScriptPublicKeyModelResponseSchema? right)
                => !(left == right);
        }
    }
}
