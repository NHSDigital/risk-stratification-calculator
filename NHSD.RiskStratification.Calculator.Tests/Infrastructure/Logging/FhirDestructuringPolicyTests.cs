using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.RiskStratification.Calculator.FhirJson;
using NHSD.RiskStratification.Calculator.Infrastructure.Logging;
using Serilog.Events;
using Serilog.Formatting.Json;
using Xunit;

namespace NHSD.RiskStratification.Calculator.Tests.Infrastructure.Logging
{
    public class FhirDestructuringPolicyTests
    {
        [Fact]
        public void Should_output_same_as_fhir_json()
        {
            var value = new RiskAssessment
            {
                Meta = new Meta
                {
                    VersionId = "test v1"
                },
                Status = ObservationStatus.Final,
                Subject = new ResourceReference("#")
            };

            var destructor = new FhirDestructuringPolicy();

            destructor.TryDestructure(value, null, out var result).Should().BeTrue();

            SerilogJson(result).Should().Be(FhirJson(value));
        }

        private static string FhirJson(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private static string SerilogJson(LogEventPropertyValue result)
        {
            // Default value formatter used by Serilog CompactJsonFormatter
            var formatter = new JsonValueFormatter("$type");

            using var output = new StringWriter();

            formatter.Format(result, output);

            return output.ToString();
        }
    }
}
