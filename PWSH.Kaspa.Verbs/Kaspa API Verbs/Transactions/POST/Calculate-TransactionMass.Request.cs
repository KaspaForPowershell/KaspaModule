using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class CalculateTransactionMass 
    {
        public sealed class RequestSchema : IEquatable<RequestSchema>
        {
            [JsonPropertyName("version")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Version { get; set; }

            [JsonPropertyName("inputs")]
            public List<TransactionInputRequestSchema>? Inputs { get; set; }

            [JsonPropertyName("outputs")]
            public List<TransactionOutputRequestSchema>? Outputs { get; set; }

            [JsonPropertyName("lockTime")]
            public ulong LockTime { get; set; }

            [JsonPropertyName("subnetworkId")]
            public string? SubnetworkID { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(RequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Version == other.Version &&
                    Inputs.CompareList(other.Inputs) &&
                    Outputs.CompareList(other.Outputs) &&
                    LockTime == other.LockTime &&
                    SubnetworkID.CompareString(other.SubnetworkID);
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
            => Equals(obj as RequestSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(Version);
                hash = Inputs.GenerateHashCode(hash);
                hash = Outputs.GenerateHashCode(hash);
                return HashCode.Combine(hash, LockTime, SubnetworkID);
            }

            public override string ToString()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(RequestSchema? left, RequestSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(RequestSchema? left, RequestSchema? right)
                => !(left == right);
        }

        public sealed class TransactionOutputRequestSchema : IEquatable<TransactionOutputRequestSchema>
        {
            [JsonPropertyName("amount")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Amount { get; set; }

            [JsonPropertyName("scriptPublicKey")]
            public ScriptPublicKeyRequestSchema? ScriptPublicKey { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(TransactionOutputRequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Amount == other.Amount &&
                    ScriptPublicKey == other.ScriptPublicKey;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
            => Equals(obj as TransactionOutputRequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(Amount, ScriptPublicKey);

            public override string ToString()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(TransactionOutputRequestSchema? left, TransactionOutputRequestSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(TransactionOutputRequestSchema? left, TransactionOutputRequestSchema? right)
                => !(left == right);
        }

        public sealed class ScriptPublicKeyRequestSchema : IEquatable<ScriptPublicKeyRequestSchema>
        {
            [JsonPropertyName("version")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Version { get; set; }

            [JsonPropertyName("scriptPublicKey")]
            public string? ScriptPublicKey { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ScriptPublicKeyRequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Version == other.Version &&
                    ScriptPublicKey.CompareString(other.ScriptPublicKey);
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
            => Equals(obj as ScriptPublicKeyRequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(Version, ScriptPublicKey);

            public override string ToString()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(ScriptPublicKeyRequestSchema? left, ScriptPublicKeyRequestSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(ScriptPublicKeyRequestSchema? left, ScriptPublicKeyRequestSchema? right)
                => !(left == right);
        }

        public sealed class TransactionInputRequestSchema : IEquatable<TransactionInputRequestSchema>
        {
            [JsonPropertyName("previousOutpoint")]
            public PreviousOutpointRequestSchema? PreviousOutpoint { get; set; }

            [JsonPropertyName("signatureScript")]
            public string? SignatureScript { get; set; }

            [JsonPropertyName("sequence")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Sequence { get; set; }

            [JsonPropertyName("sigOpCount")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint SigOpCount { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(TransactionInputRequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    PreviousOutpoint == other.PreviousOutpoint &&
                    SignatureScript.CompareString(other.SignatureScript) &&
                    Sequence == other.Sequence &&
                    SigOpCount == other.SigOpCount;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
            => Equals(obj as TransactionInputRequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(PreviousOutpoint, SignatureScript, Sequence, SigOpCount);

            public override string ToString()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(TransactionInputRequestSchema? left, TransactionInputRequestSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(TransactionInputRequestSchema? left, TransactionInputRequestSchema? right)
                => !(left == right);
        }

        public sealed class PreviousOutpointRequestSchema : IEquatable<PreviousOutpointRequestSchema>
        {
            [JsonPropertyName("transactionId")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("index")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Index { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(PreviousOutpointRequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    TransactionID.CompareString(other.TransactionID) &&
                    Index == other.Index;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
            => Equals(obj as PreviousOutpointRequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(TransactionID, Index);

            public override string ToString()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(PreviousOutpointRequestSchema? left, PreviousOutpointRequestSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(PreviousOutpointRequestSchema? left, PreviousOutpointRequestSchema? right)
                => !(left == right);
        }
    }
}
