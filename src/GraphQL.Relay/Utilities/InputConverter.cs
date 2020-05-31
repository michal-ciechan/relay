using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GraphQL.Relay.Utilities
{
    public class InputConverter : JsonConverter<Inputs>
    {
        public override Inputs Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            // Use JsonElement as fallback.
            // Newtonsoft uses JArray or JObject.
            using JsonDocument document = JsonDocument.ParseValue(ref reader);

            var element = document.RootElement;

            var result = GetValue(element);


            var dictionary = result as Dictionary<string, object>;

            return new Inputs(dictionary ?? new Dictionary<string, object>());
        }

        public override void Write(Utf8JsonWriter writer, Inputs value, JsonSerializerOptions options)
        {
            throw new NotSupportedException($"{GetType().Name} should only be used while deserializing.");
        }

        private static object GetValue(JsonElement value)
        {
            switch (value.ValueKind)
            {
                case JsonValueKind.Object:
                {
                    var output = new Dictionary<string, object>();

                    foreach (var kvp in value.EnumerateObject())
                    {
                        output.Add(kvp.Name, GetValue(kvp.Value));
                    }

                    return output;
                }

                case JsonValueKind.Array:
                    return value.EnumerateArray().Select(GetValue).ToArray();
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                case JsonValueKind.String:
                    return value.GetString();
                case JsonValueKind.Number:
                    return value.GetDecimal();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    return value.GetRawText();
            }
        }
    }
}
