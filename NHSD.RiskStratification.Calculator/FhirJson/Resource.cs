#nullable enable
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    [JsonConverter(typeof(JsonConverterResource))]
    public abstract class Resource
    {
        private string? _id;

        [JsonProperty("resourceType")]
        public virtual string ResourceType => GetType().Name;

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string? Id
        {
            get => _id;
            set => _id = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
        public Meta? Meta { get; set; }
    }
}
