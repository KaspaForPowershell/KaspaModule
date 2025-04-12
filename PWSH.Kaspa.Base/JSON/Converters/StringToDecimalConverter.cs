using System.Text.Json.Serialization;
using System.Text.Json;

namespace PWSH.Kaspa.Base.JSON.Converters
{
    public class StringToDecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options)
        {
            decimal uintValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (stringValue is null || stringValue.Equals(string.Empty)) return decimal.Zero; 
                    if (decimal.TryParse(stringValue, out uintValue)) return uintValue;
                    break;

                case JsonTokenType.Number:
                    if (reader.TryGetDecimal(out uintValue)) return uintValue;
                    break;

                case JsonTokenType.Null:
                    return decimal.Zero;
            }

            throw new JsonException($"Invalid value for double: {reader.GetString() ?? "null"}");
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}
