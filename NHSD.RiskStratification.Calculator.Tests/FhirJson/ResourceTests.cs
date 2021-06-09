using FluentAssertions;
using Hl7.Fhir.Serialization;
using Newtonsoft.Json;
using NHSD.RiskStratification.Calculator.Coding;
using NHSD.RiskStratification.Calculator.FhirJson;
using Xunit;

namespace NHSD.RiskStratification.Calculator.Tests.FhirJson
{
    public class ResourceTests
    {
        [Theory]
        [MemberData(nameof(Examples))]
        public void Can_serialize_to_the_fhir_json(Resource example)
        {
            var json = JsonConvert.SerializeObject(example, Formatting.Indented);

            var validatedJson = EnsureValidFhirJson(json);

            json.Should().Be(validatedJson);
        }

        [Theory]
        [MemberData(nameof(Examples))]
        public void Can_deserialize_from_json_representation(Resource example)
        {
            var json = JsonConvert.SerializeObject(example, Formatting.Indented);

            var roundTripJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<Resource>(json), Formatting.Indented);

            roundTripJson.Should().Be(json);
        }

        // Round trip json using fhir lib to ensure valid
        private static string EnsureValidFhirJson(string json) => new FhirJsonSerializer(new SerializerSettings { Pretty = true }).SerializeToString(new FhirJsonParser().Parse(json));

        public static TheoryData<Resource> Examples => new TheoryData<Resource>
        {
            new OperationOutcome
            {
                Id = "101",
                Meta = new Meta
                {
                    VersionId = "123"
                },
                Issue =
                {
                    new OperationOutcome.IssueComponent
                    {
                        Severity = OperationOutcome.IssueSeverity.Error,
                        Code = OperationOutcome.IssueType.Invalid,
                        Details = new CodeableConcept("test", "test")
                    }
                }
            },
            new OperationOutcome
            {
                Id = "with null, empty, and whitespace expression",
                Issue =
                {
                    new OperationOutcome.IssueComponent
                    {
                       Code = OperationOutcome.IssueType.Invalid,
                       Expression =new [] {"  ", null, string.Empty}
                    }
                }
            },
            new ObservationString
            {
                Value = "That's a big one"
            },
            new ObservationString
            {
                Id = "missing value"
            },
            new ObservationString
            {
                Id = "empty value",
                Value = string.Empty
            },
            new ObservationString
            {
                Id = "whitespace value",
                Value = "    "
            },
            new ObservationQuantity
            {
                Id = "height",
                Code = SnomedCodingSystem.Height,
                Value = new Quantity(170, "cm")
            },
            new ObservationCodeableConcept
            {
                Id = "Housing",
                Code = SnomedCodingSystem.Housing,
                Value = SnomedCodingSystem.Housing_Homeless
            },
            new Bundle
            {
                Id = "test-bundle",
                Type = Bundle.BundleType.Transaction,
                Entry =
                {
                    new Bundle.EntryComponent
                    {
                        Resource =  new ObservationQuantity
                        {
                            Id = "height",
                            Code = SnomedCodingSystem.Height,
                            Value = new Quantity(170, "cm")
                        },
                    }
                }
            },
            new Bundle
            {
                Id = "no-entries",
            },
            new  RiskAssessment
            {
                Prediction =
                {
                    new RiskAssessment.PredictionComponent
                    {
                        Outcome = new CodeableConcept("http://snomed.info/sct", "419620001" ,  "419620001", "Death attributed to COVID-19"),
                        Probability =  0.00563m,
                        RelativeRisk =  2.85641806189751m
                    },
                }
            },
        };
    }
}
