#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class RiskAssessment : Resource
    {
        private List<PredictionComponent>? _prediction;
        private List<Resource>? _contained;
        private List<ResourceReference>? _basis;

        [JsonProperty("status", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public ObservationStatus? Status { get; set; }

        [JsonProperty("prediction", Order = 1)]
        public List<PredictionComponent> Prediction
        {
            get => _prediction ??= new List<PredictionComponent>();
            set => _prediction = value;
        }

        /// <summary>
        /// Convention used by <see cref="Newtonsoft"/> to control serialisation of <see cref="Prediction"/>
        /// </summary>
        public bool ShouldSerializePrediction() => _prediction?.Count > 0;

        [JsonProperty("contained", Order = 1)]
        public List<Resource> Contained
        {
            get => _contained ??= new List<Resource>();
            set => _contained = value;
        }

        /// <summary>
        /// Convention used by <see cref="Newtonsoft"/> to control serialisation of <see cref="Contained"/>
        /// </summary>
        public bool ShouldSerializeContained() => _contained?.Count > 0;

        [JsonProperty("basis", Order = 1)]
        public List<ResourceReference> Basis
        {
            get => _basis ??= new List<ResourceReference>();
            set => _basis = value;
        }

        /// <summary>
        /// Convention used by <see cref="Newtonsoft"/> to control serialisation of <see cref="Basis"/>
        /// </summary>
        public bool ShouldSerializeBasis() => _basis?.Count > 0;

        [JsonProperty("subject", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public ResourceReference? Subject { get; set; }

        public class PredictionComponent
        {
            [JsonProperty("outcome", NullValueHandling = NullValueHandling.Ignore)]
            public CodeableConcept? Outcome { get; set; }

            [JsonProperty("qualitativeRisk", NullValueHandling = NullValueHandling.Ignore)]
            public CodeableConcept? QualitativeRisk { get; set; }

            [JsonProperty("probabilityDecimal", NullValueHandling = NullValueHandling.Ignore)]
            public decimal? Probability { get; set; }

            [JsonProperty("relativeRisk", NullValueHandling = NullValueHandling.Ignore)]
            public decimal? RelativeRisk { get; set; }
        }
    }

    [JsonConverter(typeof(JsonConverterFhirEnum<ObservationStatus>))]
    public enum ObservationStatus
    {
        Registered,
        Preliminary,
        Final,
        Amended,
        Corrected,
        Cancelled,
        EnteredInError,
        Unknown,
    }
}
