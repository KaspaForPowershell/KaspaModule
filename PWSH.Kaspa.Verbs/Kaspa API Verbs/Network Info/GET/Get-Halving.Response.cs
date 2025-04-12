using System.Text.Json;
using System.Text.Json.Serialization;
using PWSH.Kaspa.Base;
using PWSH.Kaspa.Base.JSON.Converters;
using PWSH.Kaspa.Base.JSON.Interfaces;

namespace PWSH.Kaspa.Verbs
{
    public sealed partial class GetHalving
    {
        private sealed class ResponseSchema : IEquatable<ResponseSchema>, IJSONableDisplayable
        {
            [JsonPropertyName("nextHalvingTimestamp")]
            [JsonConverter(typeof(StringToLongConverter))]
            public long NextHalvingTimestamp { get; set; }

            [JsonPropertyName("nextHalvingDate")]
            public string? NextHalvingDate { get; set; }

            [JsonPropertyName("nextHalvingAmount")]
            [JsonConverter(typeof(StringToDecimalConverter))]
            public decimal NextHalvingAmount { get; set; }

/* -----------------------------------------------------------------
HELPERS                                                            |
----------------------------------------------------------------- */

            public bool Equals(ResponseSchema? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return
                    NextHalvingTimestamp == other.NextHalvingTimestamp &&
                    NextHalvingDate.CompareString(other.NextHalvingDate) &&
                    NextHalvingAmount == other.NextHalvingAmount;
            }

            public string ToJSON()
                => JsonSerializer.Serialize(this, KaspaModuleInitializer.Instance?.ResponseSerializer);

/* -----------------------------------------------------------------
OVERRIDES                                                          |
----------------------------------------------------------------- */

            public override bool Equals(object? obj)
                => Equals(obj as ResponseSchema);

            public override int GetHashCode()
                => HashCode.Combine(NextHalvingTimestamp, NextHalvingDate, NextHalvingAmount);

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
}
