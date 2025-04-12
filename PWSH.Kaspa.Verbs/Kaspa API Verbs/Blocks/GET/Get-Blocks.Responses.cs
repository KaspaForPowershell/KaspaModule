using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;
using PWSH.Kaspa.Verbs.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetBlocks
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("blockHashes")]
            public List<string>? BlockHashes { get; set; }

            [JsonPropertyName("blocks")]
            public List<BlockResponseSchema>? Blocks { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    BlockHashes.CompareList(other.BlockHashes) &&
                    Blocks.CompareList(other.Blocks);
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
                var hash = BlockHashes.GenerateHashCode(0);
                return Blocks.GenerateHashCode(hash);
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

        private sealed class BlockResponseSchema : IEquatable<BlockResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("header")]
            public BlockHeaderResponseSchema? Header { get; set; }

            [JsonPropertyName("transactions")]
            public List<BlockTransactionResponseSchema>? Transactions { get; set; }

            [JsonPropertyName("verboseData")]
            public BlockVerboseDataResponseSchema? VerboseData { get; set; }

            [JsonPropertyName("extra")]
            public BlockExtraResponseSchema? Extra { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Header == other.Header &&
                    Transactions.CompareList(other.Transactions) &&
                    VerboseData == other.VerboseData &&
                    Extra == other.Extra;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockResponseSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(Header);
                hash = Transactions.GenerateHashCode(hash);

                return HashCode.Combine(hash, VerboseData, Extra);
            }

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockResponseSchema? left, BlockResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(BlockResponseSchema? left, BlockResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockHeaderResponseSchema : IEquatable<BlockHeaderResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("version")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Version { get; set; }

            [JsonPropertyName("hashMerkleRoot")]
            public string? HashMerkleRoot { get; set; }

            [JsonPropertyName("acceptedIdMerkleRoot")]
            public string? AcceptedIdMerkleRoot { get; set; }

            [JsonPropertyName("utxoCommitment")]
            public string? UTXOCommitment { get; set; }

            [JsonPropertyName("timestamp")]
            [JsonConverter(typeof(StringToLongConverter))]
            public long Timestamp { get; set; }

            [JsonPropertyName("bits")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Bits { get; set; }

            [JsonPropertyName("nonce")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Nonce { get; set; }

            [JsonPropertyName("daaScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong DaaScore { get; set; }

            [JsonPropertyName("blueWork")]
            public string? BlueWork { get; set; }

            [JsonPropertyName("parents")]
            public List<BlockParentHashResponseSchema>? Parents { get; set; }

            [JsonPropertyName("blueScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlueScore { get; set; }

            [JsonPropertyName("pruningPoint")]
            public string? PruningPoint { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockHeaderResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Version == other.Version &&
                    HashMerkleRoot.CompareString(other.HashMerkleRoot) &&
                    AcceptedIdMerkleRoot.CompareString(other.AcceptedIdMerkleRoot) &&
                    UTXOCommitment.CompareString(other.UTXOCommitment) &&
                    Timestamp == other.Timestamp &&
                    Bits == other.Bits &&
                    Nonce == other.Nonce &&
                    DaaScore == other.DaaScore &&
                    BlueWork.CompareString(other.BlueWork) &&
                    Parents.CompareList(other.Parents) &&
                    BlueScore == other.BlueScore &&
                    PruningPoint.CompareString(other.PruningPoint);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockHeaderResponseSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(Version, HashMerkleRoot, AcceptedIdMerkleRoot);
                hash = HashCode.Combine(hash, UTXOCommitment, Timestamp, Bits, Nonce, DaaScore, BlueWork);
                hash = Parents.GenerateHashCode(hash);

                return HashCode.Combine(hash, BlueScore, PruningPoint);
            }

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

        private sealed class BlockVerboseDataResponseSchema : IEquatable<BlockVerboseDataResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("isHeaderOnly")]
            public bool IsHeaderOnly { get; set; }

            /// <summary>
            /// The hash of a block is its unique identifier in the block DAG.
            /// A block's hash is derived directly from the block itself using a cryptographic hash function. 
            /// That ensures that no two blocks in the DAG have the same hash, and that each hash represents only the original block from which it was derived.
            /// </summary>
            [JsonPropertyName("hash")]
            public string? Hash { get; set; }

            [JsonPropertyName("difficulty")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal Difficulty { get; set; }

            /// <summary>
            /// Every block in the block DAG (aside from the genesis) has one or more parents. 
            /// A parent is simply the hash of another block that had been added to the DAG at a prior time.
            /// A block's selected parent is the parent that has the most accumulated proof-of-work.
            /// </summary>
            [JsonPropertyName("selectedParentHash")]
            public string? SelectedParentHash { get; set; }

            [JsonPropertyName("transactionIds")]
            public List<string>? TransactionIDs { get; set; }

            [JsonPropertyName("blueScore")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlueScore { get; set; }

            /// <summary>
            /// Every block in the block DAG (aside from the blocks forming the tips) has one or more children. 
            /// A child is simply the hash of another block that has been added to the DAG at a later time and that has the block hash in its parents.
            /// </summary>
            [JsonPropertyName("childrenHashes")]
            public List<string>? ChildrenHashes { get; set; }

            [JsonPropertyName("mergeSetBluesHashes")]
            public List<string>? MergeSetBluesHashes { get; set; }

            [JsonPropertyName("mergeSetRedsHashes")]
            public List<string>? MergeSetRedsHashes { get; set; }

            [JsonPropertyName("isChainBlock")]
            public bool IsChainBlock { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockVerboseDataResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    IsHeaderOnly == other.IsHeaderOnly &&
                    Hash == other.Hash &&
                    Difficulty == other.Difficulty &&
                    SelectedParentHash == other.SelectedParentHash &&
                    TransactionIDs.CompareList(other.TransactionIDs) &&
                    BlueScore == other.BlueScore &&
                    ChildrenHashes.CompareList(other.ChildrenHashes) &&
                    MergeSetBluesHashes.CompareList(other.MergeSetBluesHashes) &&
                    MergeSetRedsHashes.CompareList(other.MergeSetRedsHashes) &&
                    IsChainBlock == other.IsChainBlock;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockVerboseDataResponseSchema);

            public override int GetHashCode()
            {
                var hash = HashCode.Combine(Hash, Difficulty, SelectedParentHash, TransactionIDs);
                hash = TransactionIDs.GenerateHashCode(hash);
                hash = HashCode.Combine(hash, BlueScore);
                hash = ChildrenHashes.GenerateHashCode(hash);
                hash = MergeSetBluesHashes.GenerateHashCode(hash);
                hash = MergeSetRedsHashes.GenerateHashCode(hash);

                return HashCode.Combine(hash, IsChainBlock);
            }

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockVerboseDataResponseSchema? left, BlockVerboseDataResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(BlockVerboseDataResponseSchema? left, BlockVerboseDataResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockExtraResponseSchema : IEquatable<BlockExtraResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("color")]
            public string? Color { get; set; }

            [JsonPropertyName("minerAddress")]
            public string? MinerAddress { get; set; }

            [JsonPropertyName("minerInfo")]
            public string? MinerInfo { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockExtraResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Color.CompareString(other.Color) &&
                    MinerAddress.CompareString(other.MinerAddress) &&
                    MinerInfo.CompareString(other.MinerInfo);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockExtraResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Color, MinerAddress, MinerInfo);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockExtraResponseSchema? left, BlockExtraResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(BlockExtraResponseSchema? left, BlockExtraResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockParentHashResponseSchema : IEquatable<BlockParentHashResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("parentHashes")]
            public List<string>? ParentHashes { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockParentHashResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return ParentHashes.CompareList(other.ParentHashes);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockParentHashResponseSchema);

            public override int GetHashCode()
                => ParentHashes.GenerateHashCode(0);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockParentHashResponseSchema? left, BlockParentHashResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(BlockParentHashResponseSchema? left, BlockParentHashResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockTransactionResponseSchema : IEquatable<BlockTransactionResponseSchema>, IJSONableDisplayable, IMassCalculable
        {
            [JsonPropertyName("outputs")]
            public List<BlockTransactionOutputResponseSchema>? Outputs { get; set; }

            [JsonPropertyName("subnetworkId")]
            public string? SubnetworkID { get; set; }

            [JsonPropertyName("payload")]
            public string? Payload { get; set; }

            [JsonPropertyName("verboseData")]
            public BlockTransactionVerboseDataResponseSchema? VerboseData { get; set; }

            [JsonPropertyName("version")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Version { get; set; }

            [JsonPropertyName("inputs")]
            public List<BlockTransactionInputResponseSchema>? Inputs { get; set; }

            [JsonPropertyName("lockTime")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong LockTime { get; set; }

            [JsonPropertyName("gas")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Gas { get; set; }

            [JsonPropertyName("mass")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Mass { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockTransactionResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Outputs.CompareList(other.Outputs) &&
                    SubnetworkID == other.SubnetworkID &&
                    Payload == other.Payload &&
                    VerboseData == other.VerboseData &&
                    Version == other.Version &&
                    Inputs.CompareList(other.Inputs) &&
                    LockTime == other.LockTime &&
                    Gas == other.Gas &&
                    Mass == other.Mass;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

            public CalculateTransactionMass.RequestSchema ToMassRequestSchema()
            {
                var request = new CalculateTransactionMass.RequestSchema()
                {
                    Version = Version,
                    SubnetworkID = SubnetworkID,
                    LockTime = LockTime
                };

                var inputs = Inputs;
                if (inputs is not null)
                {
                    request.Inputs = [];

                    foreach (var input in inputs)
                    {
                        var newRequestInput = new CalculateTransactionMass.TransactionInputRequestSchema { SignatureScript = input.SignatureScript };

                        if (input.PreviousOutpoint is not null)
                        {
                            newRequestInput.PreviousOutpoint = new()
                            {
                                Index = input.PreviousOutpoint.Index,
                                TransactionID = input.PreviousOutpoint.TransactionID
                            };
                        }

                        newRequestInput.SignatureScript = input.SignatureScript;
                        newRequestInput.Sequence = input.Sequence;
                        newRequestInput.SigOpCount = input.SigOpCount;

                        request.Inputs.Add(newRequestInput);
                    }
                }

                var outputs = Outputs;
                if (outputs is not null)
                {
                    request.Outputs = [];

                    foreach (var output in outputs)
                    {
                        var newRequestOutput = new CalculateTransactionMass.TransactionOutputRequestSchema { Amount = output.Amount };

                        if (output.ScriptPublicKey is not null)
                        {
                            newRequestOutput.ScriptPublicKey = new CalculateTransactionMass.ScriptPublicKeyRequestSchema()
                            {
                                ScriptPublicKey = output.ScriptPublicKey.ScriptPublicKey,
                                Version = output.ScriptPublicKey.Version
                            };
                        }

                        request.Outputs.Add(newRequestOutput);
                    }
                }

                return request;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockTransactionResponseSchema);

            public override int GetHashCode()
            {
                var hash = 17;

                if (Outputs != null)
                {
                    foreach (var output in Outputs) hash = HashCode.Combine(hash, output);
                }

                hash = HashCode.Combine(hash, SubnetworkID, Payload, VerboseData, Version);

                if (Inputs != null)
                {
                    foreach (var input in Inputs) hash = HashCode.Combine(hash, input);
                }

                return HashCode.Combine(hash, LockTime, Gas, Mass);
            }

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockTransactionResponseSchema? left, BlockTransactionResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(BlockTransactionResponseSchema? left, BlockTransactionResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockTransactionVerboseDataResponseSchema : IEquatable<BlockTransactionVerboseDataResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("transactionId")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("hash")]
            public string? Hash { get; set; }

            [JsonPropertyName("blockHash")]
            public string? BlockHash { get; set; }

            [JsonPropertyName("blockTime")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong BlockTime { get; set; }

            [JsonPropertyName("computeMass")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong ComputeMass { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockTransactionVerboseDataResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    TransactionID.CompareString(other.TransactionID) &&
                    Hash.CompareString(other.Hash) &&
                    BlockHash.CompareString(other.BlockHash) &&
                    BlockTime == other.BlockTime &&
                    ComputeMass == other.ComputeMass;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockTransactionVerboseDataResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(TransactionID, Hash, BlockHash, BlockTime, ComputeMass);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockTransactionVerboseDataResponseSchema? left, BlockTransactionVerboseDataResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(BlockTransactionVerboseDataResponseSchema? left, BlockTransactionVerboseDataResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockTransactionInputResponseSchema : IEquatable<BlockTransactionInputResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("previousOutpoint")]
            public PreviousOutpointResponseSchema? PreviousOutpoint { get; set; }

            [JsonPropertyName("signatureScript")]
            public string? SignatureScript { get; set; }

            [JsonPropertyName("sigOpCount")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint SigOpCount { get; set; }

            [JsonPropertyName("sequence")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Sequence { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockTransactionInputResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    PreviousOutpoint == other.PreviousOutpoint &&
                    SignatureScript.CompareString(other.SignatureScript) &&
                    SigOpCount == other.SigOpCount &&
                    Sequence == other.Sequence;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockTransactionInputResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(PreviousOutpoint, SignatureScript, SigOpCount, Sequence);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockTransactionInputResponseSchema? left, BlockTransactionInputResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(BlockTransactionInputResponseSchema? left, BlockTransactionInputResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockTransactionOutputResponseSchema : IEquatable<BlockTransactionOutputResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("amount")]
            [JsonConverter(typeof(StringToUlongConverter))]
            public ulong Amount { get; set; }

            [JsonPropertyName("scriptPublicKey")]
            public ScriptPublicKeyResponseSchema? ScriptPublicKey { get; set; }

            [JsonPropertyName("verboseData")]
            public BlockOutputVerboseDataResponseSchema? VerboseData { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockTransactionOutputResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Amount == other.Amount &&
                    ScriptPublicKey == other.ScriptPublicKey &&
                    VerboseData == other.VerboseData;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockTransactionOutputResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Amount, ScriptPublicKey, VerboseData);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockTransactionOutputResponseSchema? left, BlockTransactionOutputResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(BlockTransactionOutputResponseSchema? left, BlockTransactionOutputResponseSchema? right)
                => !(left == right);
        }

        private sealed class PreviousOutpointResponseSchema : IEquatable<PreviousOutpointResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("transactionId")]
            public string? TransactionID { get; set; }

            [JsonPropertyName("index")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Index { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(PreviousOutpointResponseSchema? other)
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
                => Equals(obj as PreviousOutpointResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(TransactionID, Index);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(PreviousOutpointResponseSchema? left, PreviousOutpointResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(PreviousOutpointResponseSchema? left, PreviousOutpointResponseSchema? right)
                => !(left == right);
        }

        private sealed class ScriptPublicKeyResponseSchema : IEquatable<ScriptPublicKeyResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("version")]
            [JsonConverter(typeof(StringToUintConverter))]
            public uint Version { get; set; }

            [JsonPropertyName("scriptPublicKey")]
            public string? ScriptPublicKey { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ScriptPublicKeyResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    Version == other.Version &&
                    ScriptPublicKey.CompareString(other.ScriptPublicKey);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ScriptPublicKeyResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(Version, ScriptPublicKey);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(ScriptPublicKeyResponseSchema? left, ScriptPublicKeyResponseSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(ScriptPublicKeyResponseSchema? left, ScriptPublicKeyResponseSchema? right)
                => !(left == right);
        }

        private sealed class BlockOutputVerboseDataResponseSchema : IEquatable<BlockOutputVerboseDataResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("scriptPublicKeyType")]
            public string? ScriptPublicKeyType { get; set; }

            [JsonPropertyName("scriptPublicKeyAddress")]
            public string? ScriptPublicKeyAddress { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(BlockOutputVerboseDataResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    ScriptPublicKeyType.CompareString(other.ScriptPublicKeyType) &&
                    ScriptPublicKeyAddress.CompareString(other.ScriptPublicKeyAddress);
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as BlockOutputVerboseDataResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(ScriptPublicKeyType, ScriptPublicKeyAddress);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(BlockOutputVerboseDataResponseSchema? left, BlockOutputVerboseDataResponseSchema? right)
            {
                if (left is null) return right is null;

                return left.Equals(right);
            }

            public static bool operator !=(BlockOutputVerboseDataResponseSchema? left, BlockOutputVerboseDataResponseSchema? right)
                => !(left == right);
        }
    }
}
