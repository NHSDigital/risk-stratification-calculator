namespace NHSD.RiskStratification.Calculator.Tests.FhirJson
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using Hl7.Fhir.Serialization;
    using NHSD.RiskStratification.Calculator.FhirJson;
    using Xunit;

    public class CodingTests
    {
        [Theory]
        [MemberData(nameof(Examples))]
        public void Can_serialize_to_the_fhir_json(Coding example)
        {
            var json = JsonConvert.SerializeObject(example);

            var validatedJson = EnsureValidFhirJson(json);

            json.Should().Be(validatedJson);
        }

        [Theory]
        [MemberData(nameof(Examples))]
        public void Can_deserialize_from_fhir_json_representation(Coding example)
        {
            var json = JsonConvert.SerializeObject(example);

            var validatedJson = EnsureValidFhirJson(json);

            var roundTripJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Coding>(validatedJson));

            roundTripJson.Should().Be(json);
        }

        // Round trip json using fhir lib to ensure valid
        private static string EnsureValidFhirJson(string json) => new FhirJsonSerializer().SerializeToString(new FhirJsonParser().Parse<Hl7.Fhir.Model.Coding>(json));

        public static TheoryData<Coding> Examples => new TheoryData<Coding>
        {
            default,
            new Coding("http://snomed.info/sct", "424144002"),
            new Coding("http://nhsd.riskstrat.nhs/qcovid", "CORONARY_HEART_DISEASE", "Coronary heart disease")
        };
    }
}
