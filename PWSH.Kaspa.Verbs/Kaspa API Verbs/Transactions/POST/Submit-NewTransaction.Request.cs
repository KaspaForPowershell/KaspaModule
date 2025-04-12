using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class SubmitNewTransaction
    {
        private sealed class RequestSchema : IEquatable<RequestSchema>
        {
            [JsonPropertyName("transaction")]
            public TransactionRequestSchema? Transaction { get; set; }

            [JsonPropertyName("allowOrphan")]
            public bool AllowOrphan { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(RequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Transaction == other.Transaction &&
                    AllowOrphan == other.AllowOrphan;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as RequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(Transaction, AllowOrphan);

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

        public sealed class TransactionRequestSchema : IEquatable<TransactionRequestSchema>
        {
            [JsonPropertyName("version")]
            public uint Version { get; set; }

            [JsonPropertyName("inputs")]
            public List<TransactionInputRequestSchema>? Inputs { get; set; }

            [JsonPropertyName("outputs")]
            public List<TransactionOutputRequestSchema>? Outputs { get; set; }

            [JsonPropertyName("lockTime")]
            public int LockTime { get; set; }

            [JsonPropertyName("subnetworkId")]
            public string? SubnetworkID { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(TransactionRequestSchema? other)
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
                => Equals(obj as TransactionRequestSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(17, Version);
                hash = Inputs.GenerateHashCode(hash);
                hash = Outputs.GenerateHashCode(hash);
                return HashCode.Combine(hash, LockTime, SubnetworkID);
            }

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(TransactionRequestSchema? left, TransactionRequestSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(TransactionRequestSchema? left, TransactionRequestSchema? right)
                => !(left == right);
        }

        public class TransactionInputRequestSchema : IEquatable<TransactionInputRequestSchema>
        {
            [JsonPropertyName("previousOutpoint")]
            public OutpointRequestSchema? PreviousOutpoint { get; set; }

            [JsonPropertyName("signatureScript")]
            public string? SignatureScript { get; set; }

            [JsonPropertyName("sequence")]
            public int Sequence { get; set; }

            [JsonPropertyName("sigOpCount")]
            public int SigOpCount { get; set; }

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

        public sealed class OutpointRequestSchema : IEquatable<OutpointRequestSchema>
        {
            [JsonPropertyName("transactionId")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("index")]
            public int Index { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(OutpointRequestSchema? other)
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
                => Equals(obj as OutpointRequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(TransactionID, Index);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(OutpointRequestSchema? left, OutpointRequestSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(OutpointRequestSchema? left, OutpointRequestSchema? right)
                => !(left == right);
        }

        public sealed class TransactionOutputRequestSchema : IEquatable<TransactionOutputRequestSchema>
        {
            [JsonPropertyName("amount")]
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
    }
}
