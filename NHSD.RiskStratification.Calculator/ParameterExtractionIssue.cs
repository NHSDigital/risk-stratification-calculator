using System.Collections.Generic;
using System.Linq;
using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator
{
    public class ParameterExtractionIssue
    {
        public ParameterExtractionIssue(string message, OperationOutcome.IssueType issueType, CodeableConcept? observationCode = null, params string[] observationIds)
        {
            Message = message;
            IssueType = issueType;
            ObservationCode = observationCode;
            ObservationIds = observationIds;
        }

        public CodeableConcept? ObservationCode { get; }

        public IReadOnlyList<string> ObservationIds { get; }

        public OperationOutcome.IssueType IssueType { get; }

        public string Message { get; }

        public OperationOutcome.IssueComponent AsFhirElement()
        {
            var message = Message;

            if (ObservationCode.HasValue)
            {
                message = $"Observation of CodeableConcept[{ObservationCode.Value.Coding.First().Code}] using system [{ObservationCode.Value.Coding.First().System}] {message}";
            }

            return new OperationOutcome.IssueComponent
            {
                Severity = OperationOutcome.IssueSeverity.Error,
                Code = IssueType,
                Diagnostics = message,
                Expression = ObservationIds?.Select(id => $"#{id}").ToArray()
            };
        }


    }
}
