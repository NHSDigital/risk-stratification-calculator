#nullable enable
using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator
{
    /// <summary>
    /// Maintains the search term as context for search results so that it can be used when logging issues.
    /// </summary>
    public readonly struct ObservationSearchResult
    {
        public ObservationSearchResult(CodeableConcept searchTerm, Observation? observation)
        {
            SearchTerm = searchTerm;
            Observation = observation;
        }

        public CodeableConcept SearchTerm { get; }
        public Observation? Observation { get; }

        public bool HasObservation => Observation != null;

        public void Deconstruct(out CodeableConcept searchTerm, out Observation? observation)
        {
            searchTerm = SearchTerm;
            observation = Observation;
        }
    }
}
