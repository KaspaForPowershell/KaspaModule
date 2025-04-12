using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetAddressesActive
    {
        private sealed class RequestSchema : IEquatable<RequestSchema>
        {
            [JsonPropertyName("addresses")]
            public List<string>? Addresses { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(RequestSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return Addresses.CompareList(other.Addresses);
            }

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as RequestSchema);

            public override int GetHashCode()
                => Addresses.GenerateHashCode(17);

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
    }
}
