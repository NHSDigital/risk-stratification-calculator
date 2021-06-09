#nullable  enable
using System;
using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.Tests
{
    public static class FhirJsonObjectAssertionsExtensions
    {
        public static AndConstraint<ObjectAssertions> HaveSameFhirJsonAs(this ObjectAssertions assertions, object expected)
        {
            var actualJson = JsonConvert.SerializeObject(assertions.Subject, Formatting.Indented, FhirConverter.Instance);
            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented, FhirConverter.Instance);

            actualJson.Should().Be(expectedJson);

            return new AndConstraint<ObjectAssertions>(assertions);
        }

        public static AndConstraint<GenericCollectionAssertions<T>> HaveSameFhirJsonAs<T>(this GenericCollectionAssertions<T> assertions, object expected)
        {
            var actualJson = JsonConvert.SerializeObject(assertions.Subject, Formatting.Indented, FhirConverter.Instance);
            var expectedJson = JsonConvert.SerializeObject(expected, Formatting.Indented, FhirConverter.Instance);

            actualJson.Should().Be(expectedJson);

            return new AndConstraint<GenericCollectionAssertions<T>>(assertions);
        }

        /// <summary>
        /// If the object is a Fhir object then using the Fhir serialiser.
        /// </summary>
        private class FhirConverter : JsonConverter
        {
            public static readonly FhirConverter Instance = new FhirConverter();

            public override bool CanConvert(Type objectType)
            {
                return typeof(Base).IsAssignableFrom(objectType);
            }

            public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
            {
                if (value is Base fhirBase)
                {
                    fhirBase.WriteTo(writer);
                    return;
                }

                throw new NotImplementedException();
            }

            public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
            {

                throw new NotImplementedException();
            }
        }
    }
}
