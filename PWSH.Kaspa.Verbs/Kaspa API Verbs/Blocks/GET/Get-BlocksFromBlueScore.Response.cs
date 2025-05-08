namespace PWSH.Kaspa.Verbs;

public sealed partial class GetBlocksFromBlueScore
{
    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ResponseSchema
    {
        [JsonPropertyName("header")]
        public BlockHeaderResponseSchema? Header { get; set; }

        [JsonPropertyName("transactions")]
        public List<BlockTransactionResponseSchema>? Transactions { get; set; }

        [JsonPropertyName("verboseData")]
        public BlockVerboseDataResponseSchema? VerboseData { get; set; }

        [JsonPropertyName("extra")]
        public BlockExtraResponseSchema? Extra { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockHeaderResponseSchema
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
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockVerboseDataResponseSchema
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
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockExtraResponseSchema
    {
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("minerAddress")]
        public string? MinerAddress { get; set; }

        [JsonPropertyName("minerInfo")]
        public string? MinerInfo { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockParentHashResponseSchema
    {
        [JsonPropertyName("parentHashes")]
        public List<string>? ParentHashes { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockTransactionResponseSchema : IMassCalculable
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
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockTransactionVerboseDataResponseSchema
    {
        [JsonPropertyName("transactionId")]
        public string? TransactionId { get; set; }

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
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockTransactionInputResponseSchema
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
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockTransactionOutputResponseSchema
    {
        [JsonPropertyName("amount")]
        [JsonConverter(typeof(StringToUlongConverter))]
        public ulong Amount { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public ScriptPublicKeyResponseSchema? ScriptPublicKey { get; set; }

        [JsonPropertyName("verboseData")]
        public BlockOutputVerboseDataResponseSchema? VerboseData { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class PreviousOutpointResponseSchema
    {
        [JsonPropertyName("transactionId")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("index")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Index { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class ScriptPublicKeyResponseSchema
    {
        [JsonPropertyName("version")]
        [JsonConverter(typeof(StringToUintConverter))]
        public uint Version { get; set; }

        [JsonPropertyName("scriptPublicKey")]
        public string? ScriptPublicKey { get; set; }
    }

    [GenerateResponseSchemaBoilerplate]
    private sealed partial class BlockOutputVerboseDataResponseSchema
    {
        [JsonPropertyName("scriptPublicKeyType")]
        public string? ScriptPublicKeyType { get; set; }

        [JsonPropertyName("scriptPublicKeyAddress")]
        public string? ScriptPublicKeyAddress { get; set; }
    }
}
