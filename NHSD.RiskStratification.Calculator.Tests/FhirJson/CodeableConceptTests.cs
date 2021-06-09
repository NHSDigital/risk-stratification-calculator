namespace NHSD.RiskStratification.Calculator.Tests.FhirJson
{
    using FluentAssertions;
    using Hl7.Fhir.Serialization;
    using Newtonsoft.Json;
    using NHSD.RiskStratification.Calculator.FhirJson;
    using Xunit;

    public class CodeableConceptTests
    {
        [Theory]
        [MemberData(nameof(Examples))]
        public void Can_serialize_to_the_fhir_json(CodeableConcept example)
        {
            var json = JsonConvert.SerializeObject(example);

            var validatedJson = EnsureValidFhirJson(json);

            json.Should().Be(validatedJson);
        }

        [Theory]
        [MemberData(nameof(Examples))]
        public void Can_deserialize_from_fhir_json_representation(CodeableConcept example)
        {
            var json = JsonConvert.SerializeObject(example);

            var validatedJson = EnsureValidFhirJson(json);

            var roundTripJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<CodeableConcept>(validatedJson));

            json.Should().Be(roundTripJson);
        }

        // Round trip json using fhir lib to ensure valid
        private static string EnsureValidFhirJson(string json) => new FhirJsonSerializer().SerializeToString(new FhirJsonParser().Parse<Hl7.Fhir.Model.CodeableConcept>(json));

        public static TheoryData<CodeableConcept> Examples => new TheoryData<CodeableConcept>
        {
            new CodeableConcept("http://snomed.info/sct", "424144002"),
            new CodeableConcept("http://snomed.info/sct", "424144002", "Age"),
            new CodeableConcept
            (
                new[]
                {
                    new Coding("http://snomed.info/sct", "260385009", "Negative"),
                    new Coding("https://acme.lab/resultcodes", "NEG", "Negative")
                },
                "Negative for Chlamydia Trachomatis rRNA"
            )
        };
    }
}
