namespace PWSH.Kaspa.Verbs;

public sealed partial class SubmitNewTransaction
{
    private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
    {
        [JsonPropertyName("transaction_id")]
        public string? TransactionID { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

        public bool Equals(ResponseSchema? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                TransactionID.CompareString(other.TransactionID) &&
                Error.CompareString(other.Error);
        }

        public string ToJSON()
            => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

        public override bool Equals(object? obj)
        => Equals(obj as ResponseSchema);

        public override int GetHashCode()
            => HashCode.Combine(TransactionID, Error);

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
