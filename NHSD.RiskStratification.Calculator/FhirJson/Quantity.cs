#nullable  enable
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class Quantity
    {
        public Quantity(decimal? value, string? unit, string? system = "http://unitsofmeasure.org") : this(value, unit, unit, system)
        {
        }

        [JsonConstructor]
        public Quantity(decimal? value, string? unit, string? code, string? system)
        {
            Value = value;
            Unit = !string.IsNullOrWhiteSpace(unit) ? unit : null;
            Code = !string.IsNullOrWhiteSpace(code) ? code : null;
            System = !string.IsNullOrWhiteSpace(system) ? system : null;
        }

        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Value { get; }

        [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
        public string? Unit { get; }

        [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
        public string? System { get; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string? Code { get; }
    }
}
