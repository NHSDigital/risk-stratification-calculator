using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NHSD.RiskStratification.Calculator.FhirJson;
using Serilog.Core;
using Serilog.Events;

namespace NHSD.RiskStratification.Calculator.Infrastructure.Logging
{
    public class FhirDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            result = value switch
            {
                Resource _ => Destructure(JObject.FromObject(value)),
                RiskAssessment.PredictionComponent _ => Destructure(JObject.FromObject(value)),
                OperationOutcome.IssueComponent _ => Destructure(JObject.FromObject(value)),
                _ => null
            };

            return result != null;
        }

        private static LogEventPropertyValue Destructure(JToken jToken)
        {
            return jToken switch
            {
                JObject obj => new StructureValue(DestructureProperties(obj.Properties())),
                JArray array => new SequenceValue(array.Select(Destructure)),
                JValue value => new ScalarValue(value.Value),
                _ => new ScalarValue(null)
            };
        }

        private static IEnumerable<LogEventProperty> DestructureProperties(IEnumerable<JProperty> properties)
        {
            foreach (var jProperty in properties)
            {
                if (jProperty.Name == "probabilityDecimal")
                {
                    // Add this property twice once with lower case name to ensure backwards compatibility with bug in old fhir destructuring.
                    yield return new LogEventProperty("probabilitydecimal", Destructure(jProperty.Value));
                }

                yield return new LogEventProperty(jProperty.Name, Destructure(jProperty.Value));
            }
        }
    }
}
