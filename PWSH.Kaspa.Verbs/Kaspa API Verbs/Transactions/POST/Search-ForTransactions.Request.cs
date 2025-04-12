using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class SearchForTransactions
    {
        private sealed class RequestSchema : IEquatable<RequestSchema>
        {
            [JsonPropertyName("transactionIds")]
            public List<string>? TransactionIDs { get; set; }

            [JsonPropertyName("acceptingBlueScores")]
            public AcceptingBlueScoreRequestSchema? AcceptingBlueScores { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(RequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    TransactionIDs.CompareList(other.TransactionIDs) &&
                    AcceptingBlueScores == other.AcceptingBlueScores;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as RequestSchema);

            public override int GetHashCode()
            {
                var hash = TransactionIDs.GenerateHashCode(0);
                return HashCode.Combine(hash, AcceptingBlueScores);
            }

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

        private sealed class AcceptingBlueScoreRequestSchema : IEquatable<AcceptingBlueScoreRequestSchema>
        {
            [JsonPropertyName("gte")]
            public ulong Gte { get; set; } = 0;

            [JsonPropertyName("lt")]
            public ulong Lt { get; set; } = 0;

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(AcceptingBlueScoreRequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return Gte == other.Gte && Lt == other.Lt;
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as AcceptingBlueScoreRequestSchema);

            public override int GetHashCode()
                => HashCode.Combine(Gte, Lt);

/* -----------------------------------------------------------------
OPERATOR                                                           |
----------------------------------------------------------------- */

            public static bool operator ==(AcceptingBlueScoreRequestSchema? left, AcceptingBlueScoreRequestSchema? right)
            {
                if (left is null) return right is null;
                return left.Equals(right);
            }

            public static bool operator !=(AcceptingBlueScoreRequestSchema? left, AcceptingBlueScoreRequestSchema? right)
                => !(left == right);
        }
    }
}
