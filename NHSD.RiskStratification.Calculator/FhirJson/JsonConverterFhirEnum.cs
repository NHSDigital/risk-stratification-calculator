#nullable  enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class JsonConverterFhirEnum<T> : JsonConverter<T> where T : struct, Enum
    {
        private static readonly Dictionary<string, T> _lookup;
        private static readonly Dictionary<T, string> _names;

        static JsonConverterFhirEnum()
        {
            var values = (T[])Enum.GetValues(typeof(T));
            _lookup = values.ToDictionary(value => KebabCase(value.ToString()));
            _names = _lookup.ToDictionary(x => x.Value, x => x.Key);
        }

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
        {
            if (_names.TryGetValue(value, out var name))
            {
                writer.WriteValue(name);
            }
            else
            {
                writer.WriteNull();
            }
        }

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var name = reader.Value?.ToString();

                if (name is { } && _lookup.TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            return default;
        }

        private static string KebabCase(string value)
        {
            return Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "-$1").ToLower();
        }
    }
}
