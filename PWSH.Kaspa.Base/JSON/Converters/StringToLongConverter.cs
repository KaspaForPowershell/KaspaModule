using System.Text.Json.Serialization;
using System.Text.Json;

namespace PWSH.Kaspa.Base.JSON.Converters
{
    public class StringToLongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options)
        {
            long longValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (stringValue is null || stringValue.Equals(string.Empty)) return 0; 
                    if (long.TryParse(stringValue, out longValue)) return longValue;
                    break;

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out longValue)) return longValue;
                    break;

                case JsonTokenType.Null:
                    return 0;
            }

            throw new JsonException($"Invalid value for long: {reader.GetString() ?? "null"}");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}
