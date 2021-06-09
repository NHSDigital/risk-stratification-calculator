#nullable enable
using System.Text;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public readonly struct Coding
    {
        [JsonConstructor]
        public Coding(string? system, string? code, string? display = null)
        {
            System = !string.IsNullOrWhiteSpace(system) ? system : null;
            Code = !string.IsNullOrWhiteSpace(code) ? code : null;
            Display = !string.IsNullOrWhiteSpace(display) ? display : null;
        }

        [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
        public string? System { get; }

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public string? Code { get; }

        [JsonProperty("display", NullValueHandling = NullValueHandling.Ignore)]
        public string? Display { get; }

        public override string ToString()
        {
            var txt = new StringBuilder();

            if (Display != null)
            {
                txt.Append(Display);
            }

            if (Code != null)
            {
                if (txt.Length > 0)
                {
                    txt.Append(" ");
                }

                txt.Append(Code);
            }

            if (System != null)
            {
                if (txt.Length > 0)
                {
                    txt.Append(" ");
                }

                txt.Append("(").Append(System).Append(")");
            }

            return txt.ToString();
        }
    }
}
