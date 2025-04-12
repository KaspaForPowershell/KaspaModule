using System.Text.Json.Serialization;
using System.Text.Json;

namespace PWSH.Kaspa.Base.JSON.Converters
{
    public class StringToUintConverter : JsonConverter<uint>
    {
        public override uint Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options)
        {
            uint uintValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (stringValue is null || stringValue.Equals(string.Empty)) return 0; 
                    if (uint.TryParse(stringValue, out uintValue)) return uintValue;
                    break;

                case JsonTokenType.Number:
                    if (reader.TryGetUInt32(out uintValue)) return uintValue;
                    break;

                case JsonTokenType.Null:
                    return 0;
            }

            throw new JsonException($"Invalid value for uint: {reader.GetString() ?? "null"}");
        }

        public override void Write(Utf8JsonWriter writer, uint value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}
