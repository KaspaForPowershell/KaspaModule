using System.Text.Json.Serialization;
using System.Text.Json;

namespace PWSH.Kaspa.Base.JSON.Converters
{
    public class StringToUlongConverter : JsonConverter<ulong>
    {
        public override ulong Read(ref Utf8JsonReader reader, Type type_to_convert, JsonSerializerOptions options)
        {
            ulong ulongValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (stringValue is null || stringValue.Equals(string.Empty)) return 0; 
                    if (ulong.TryParse(stringValue, out ulongValue)) return ulongValue;
                    break;

                case JsonTokenType.Number:
                    if (reader.TryGetUInt64(out ulongValue)) return ulongValue;
                    break;

                case JsonTokenType.Null:
                    return 0;
            }

            throw new JsonException($"Invalid value for ulong: {reader.GetString() ?? "null"}");
        }

        public override void Write(Utf8JsonWriter writer, ulong value, JsonSerializerOptions options)
            => writer.WriteNumberValue(value);
    }
}
