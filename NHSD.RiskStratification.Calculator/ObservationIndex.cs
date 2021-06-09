using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator
{
    public class ObservationIndex
    {
        private readonly IDictionary<string, Observation> _observationMapping;
        private readonly List<ParameterExtractionIssue> _issues = new List<ParameterExtractionIssue>();

        public ObservationIndex(IEnumerable<Observation> observations)
        {
            _observationMapping = MapObservationCodes(observations);
        }

        public IReadOnlyCollection<ParameterExtractionIssue> Issues => _issues;

        private IDictionary<string, Observation> MapObservationCodes(IEnumerable<Observation> observations)
        {
            var observationMapping = new Dictionary<string, Observation>();

            foreach (var observation in observations)
            {
                bool hasCodings = false;

                foreach (var codingKey in observation.Code?.Coding.Select(CodingKey) ?? Array.Empty<string>())
                {
                    hasCodings = true;

                    if (observationMapping.TryGetValue(codingKey, out var duplicate))
                    {
                        InvalidIssue($"Duplicate found for Observation code [{codingKey}]", observation.Code!.Value, duplicate.Id, observation.Id);
                    }
                    else
                    {
                        observationMapping.Add(codingKey, observation);
                    }
                }

                if (!hasCodings)
                {
                    IncompleteIssue($"Observation with Id [{observation.Id}]: Code must be set and populated with Coding(s)", observation.Id);
                }
            }

            return observationMapping;
        }

        public ObservationSearchResult FindObservation(CodeableConcept observationCode)
        {
            var matchedCode = observationCode.Coding.Select(CodingKey).FirstOrDefault(c => _observationMapping.ContainsKey(c));
            var observation = matchedCode != null ? _observationMapping[matchedCode] : null;

            return new ObservationSearchResult(observationCode, observation);
        }

        private string CodingKey(FhirJson.Coding coding)
        {
            return $"{coding.Code}({coding.System})";
        }

        private void IncompleteIssue(string message, params string[] observationIds)
        {
            _issues.Add(new ParameterExtractionIssue(message, OperationOutcome.IssueType.Incomplete, observationIds: observationIds));
        }

        private void InvalidIssue(string message, FhirJson.CodeableConcept observationCode, params string[] observationIds)
        {
            _issues.Add(new ParameterExtractionIssue(message, OperationOutcome.IssueType.Invalid, observationCode, observationIds));
        }
    }
}
