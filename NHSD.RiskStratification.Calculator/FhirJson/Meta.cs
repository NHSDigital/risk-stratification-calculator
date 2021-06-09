#nullable enable
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class Meta
    {
        private string? _versionId;

        [JsonProperty("versionId", NullValueHandling = NullValueHandling.Ignore)]
        public string? VersionId
        {
            get => _versionId;
            set => _versionId = string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}
