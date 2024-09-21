using System.Text.Json;

namespace PRUNStatsCommon
{
    /// <summary>
    /// Custom class needed to convert Guids from FIO payloads
    /// </summary>
    public sealed class JsonConverterGuid : System.Text.Json.Serialization.JsonConverter<Guid>
    {
        public override Guid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TryGetGuid(out Guid value))
            {
                return value;
            }

            if (reader.TryGetString(out string str))
            {
                return Guid.Parse(str);
            }

            throw new FormatException("The JSON value is not in a supported Guid format.");
        }

        public override void Write(Utf8JsonWriter writer, Guid value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }

    public static class Ext
    {
        public static bool TryGetString(this Utf8JsonReader je, out string parsed)
        {
            var (p, r) = je.TokenType switch
            {
                JsonTokenType.String => (je.GetString(), true),
                JsonTokenType.Null => (null, true),
                _ => (default, false)
            };
            parsed = p!;
            return r;
        }
    }
}
