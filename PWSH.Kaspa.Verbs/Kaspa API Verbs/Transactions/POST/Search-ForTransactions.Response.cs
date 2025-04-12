using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class SearchForTransactions
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("subnetwork_id")]
            public string? SubnetworkID { get; set; }

            [JsonPropertyName("transaction_id")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("hash")]
            public string? Hash { get; set; }

            [JsonPropertyName("mass")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Mass { get; set; }

            [JsonPropertyName("payload")]
            public string? Payload { get; set; }

            [JsonPropertyName("block_hash")]
            public List<string>? BlockHash { get; set; }

            [JsonPropertyName("block_time")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlockTime { get; set; }

            [JsonPropertyName("is_accepted")]
            public bool IsAccepted { get; set; }

            [JsonPropertyName("accepting_block_hash")]
            public string? AcceptingBlockHash { get; set; }

            [JsonPropertyName("accepting_block_blue_score")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong AcceptingBlockBlueScore { get; set; }

            [JsonPropertyName("accepting_block_time")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong AcceptingBlockTime { get; set; }

            [JsonPropertyName("inputs")]
            public List<TransactionInputResponseSchema>? Inputs { get; set; }

            [JsonPropertyName("outputs")]
            public List<TransactionOutputResponseSchema>? Outputs { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    SubnetworkID == other.SubnetworkID &&
                    TransactionID == other.TransactionID &&
                    Hash == other.Hash &&
                    Mass == other.Mass &&
                    Payload == other.Payload &&
                    BlockHash.CompareList(other.BlockHash) &&
                    BlockTime == other.BlockTime &&
                    IsAccepted == other.IsAccepted &&
                    AcceptingBlockHash == other.AcceptingBlockHash &&
                    AcceptingBlockBlueScore == other.AcceptingBlockBlueScore &&
                    AcceptingBlockTime == other.AcceptingBlockTime &&
                    Inputs.CompareList(other.Inputs) &&
                    Outputs.CompareList(other.Outputs);
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
                var hash = HashCode.Combine(SubnetworkID, TransactionID, Hash, Mass, Payload);
                hash = BlockHash.GenerateHashCode(hash);
                hash = HashCode.Combine(BlockTime, IsAccepted, AcceptingBlockHash, AcceptingBlockBlueScore, AcceptingBlockTime);
                hash = Inputs.GenerateHashCode(hash);
                return Outputs.GenerateHashCode(hash);
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

        private sealed class TransactionInputResponseSchema : IEquatable<TransactionInputResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("transaction_id")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("index")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Index { get; set; }

            [JsonPropertyName("previous_outpoint_hash")]
            public string? PreviousOutpointHash { get; set; }

            [JsonPropertyName("previous_outpoint_index")]
            public string? PreviousOutpointIndex { get; set; }

            [JsonPropertyName("previous_outpoint_resolved")]
            public TransactionOutputResponseSchema? PreviousOutpointResolved { get; set; }

            [JsonPropertyName("previous_outpoint_address")]
            public string? PreviousOutpointAddress { get; set; }

            [JsonPropertyName("previous_outpoint_amount")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong PreviousOutpointAmount { get; set; }

            [JsonPropertyName("signature_script")]
            public string? SignatureScript { get; set; }

            [JsonPropertyName("sig_op_count")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint SigOpCount { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(TransactionInputResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    TransactionID == other.TransactionID &&
                    Index == other.Index &&
                    PreviousOutpointHash == other.PreviousOutpointHash &&
                    PreviousOutpointIndex == other.PreviousOutpointIndex &&
                    PreviousOutpointResolved == other.PreviousOutpointResolved &&
                    PreviousOutpointAddress == other.PreviousOutpointAddress &&
                    PreviousOutpointAmount == other.PreviousOutpointAmount &&
                    SignatureScript == other.SignatureScript &&
                    SigOpCount == other.SigOpCount;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as TransactionInputResponseSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(TransactionID, Index);
                HashCode.Combine(hash, PreviousOutpointHash, PreviousOutpointIndex, PreviousOutpointResolved, PreviousOutpointAddress, PreviousOutpointAmount, SignatureScript, SigOpCount);
                HashCode.Combine(hash, SignatureScript, SigOpCount);
                return hash;
            }

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(TransactionInputResponseSchema? left, TransactionInputResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(TransactionInputResponseSchema? left, TransactionInputResponseSchema? right)
                => !(left == right);
        }

        private sealed class TransactionOutputResponseSchema : IEquatable<TransactionOutputResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("transaction_id")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("index")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Index { get; set; }

            [JsonPropertyName("amount")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Amount { get; set; }

            [JsonPropertyName("script_public_key")]
            public string? ScriptPublicKey { get; set; }

            [JsonPropertyName("script_public_key_address")]
            public string? ScriptPublicKeyAddress { get; set; }

            [JsonPropertyName("script_public_key_type")]
            public string? ScriptPublicKeyType { get; set; }

            [JsonPropertyName("accepting_block_hash")]
            public string? AcceptingBlockHash { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(TransactionOutputResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    TransactionID.CompareString(other.TransactionID) &&
                    Index == other.Index &&
                    Amount == other.Amount &&
                    ScriptPublicKey.CompareString(other.ScriptPublicKey) &&
                    ScriptPublicKeyAddress.CompareString(other.ScriptPublicKeyAddress) &&
                    ScriptPublicKeyType.CompareString(other.ScriptPublicKeyType) &&
                    AcceptingBlockHash.CompareString(other.AcceptingBlockHash);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as TransactionOutputResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(TransactionID, Index, Amount, ScriptPublicKey, ScriptPublicKeyAddress, ScriptPublicKeyType, AcceptingBlockHash);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(TransactionOutputResponseSchema? left, TransactionOutputResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(TransactionOutputResponseSchema? left, TransactionOutputResponseSchema? right)
                => !(left == right);
        }
    }
}
