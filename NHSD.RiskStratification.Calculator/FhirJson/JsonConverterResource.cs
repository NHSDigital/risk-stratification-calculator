using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class JsonConverterResource : JsonConverter<Resource>
    {
        public override Resource ReadJson(JsonReader reader, Type objectType, Resource existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);

            var type = GetType(json);

            if (type == null)
            {
                throw new JsonException("Could not resolve resource type");
            }

            var value = (Resource)Activator.CreateInstance(type);

            serializer.Populate(json.CreateReader(), value);

            return value;
        }

        private static Type GetType(JObject json)
        {
            if (!json.TryGetValue("resourceType", out var resourceTypeToken))
            {
                return null;
            }

            var resourceType = resourceTypeToken.Value<string>();

            if (resourceType == nameof(Observation))
            {
                if (json.ContainsKey("valueString"))
                {
                    return typeof(ObservationString);
                }

                if (json.ContainsKey("valueCodeableConcept"))
                {
                    return typeof(ObservationCodeableConcept);
                }

                if (json.ContainsKey("valueBoolean"))
                {
                    return typeof(ObservationBoolean);
                }

                if (json.ContainsKey("valueQuantity"))
                {
                    return typeof(ObservationQuantity);
                }

                if (json.ContainsKey("valueInteger"))
                {
                    return typeof(ObservationInteger);
                }

                return typeof(Observation);
            }

            return resourceType switch
            {
                nameof(Bundle) => typeof(Bundle),
                nameof(OperationOutcome) => typeof(OperationOutcome),
                nameof(RiskAssessment) => typeof(RiskAssessment),
                _ => null
            };
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, Resource value, JsonSerializer serializer) => throw new InvalidOperationException();
    }
}
