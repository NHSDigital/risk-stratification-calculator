using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class ResourceReference
    {
        private string _reference;

        public ResourceReference()
        {
        }

        public ResourceReference(string reference) => this.Reference = reference;

        [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
        public string Reference
        {
            get => _reference;
            set => _reference = string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
