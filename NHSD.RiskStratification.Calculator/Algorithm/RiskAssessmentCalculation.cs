#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator.Algorithm
{
    public class RiskAssessmentCalculation
    {
        public RiskAssessmentCalculation(AlgorithmAssembly algorithmAssembly, IEnumerable<RiskAssessment.PredictionComponent> predictions, IEnumerable<Observation> observations, IEnumerable<OperationOutcome.IssueComponent>? issues = null)
        {
            AlgorithmAssembly = algorithmAssembly;
            Predictions = predictions.ToImmutableList();
            Observations = observations.ToImmutableList();
            Issues = (issues ?? Array.Empty<OperationOutcome.IssueComponent>()).ToImmutableList();
        }

        public RiskAssessmentCalculation(AlgorithmAssembly algorithmAssembly, IEnumerable<OperationOutcome.IssueComponent> issues)
        {
            AlgorithmAssembly = algorithmAssembly;
            Predictions = Array.Empty<RiskAssessment.PredictionComponent>();
            Issues = issues.ToImmutableList();

            if (Issues.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(issues));
            }
        }

        public AlgorithmAssembly AlgorithmAssembly { get; }

        public IReadOnlyCollection<Observation> Observations { get; } = Array.Empty<Observation>();

        public IReadOnlyCollection<RiskAssessment.PredictionComponent> Predictions { get; }

        public IReadOnlyCollection<OperationOutcome.IssueComponent> Issues { get; }

        public bool TryGetAssessment([NotNullWhen(true)] out RiskAssessment? assessment, [NotNullWhen(false)] out OperationOutcome? error)
        {
            if (HasFailed(out error))
            {
                assessment = null;

                return false;
            }

            assessment = CreateAssessment();

            return true;
        }

        private bool HasFailed([NotNullWhen(true)] out OperationOutcome? error)
        {
            if (Issues.Any(x => x.Success == false))
            {
                error = new OperationOutcome
                {
                    Issue = Issues.ToList()
                };

                SetMeta(error);

                return true;
            }

            error = null;

            return false;
        }

        private void SetMeta(Resource resource)
        {
            resource.Meta = new Meta
            {
                VersionId = AlgorithmAssembly.FullName
            };
        }

        private RiskAssessment CreateAssessment()
        {
            var assessment = new RiskAssessment
            {
                Status = ObservationStatus.Final,
                Prediction = Predictions.ToList(),
                Contained = new List<Resource>(Observations),
                Basis = Observations.Select(o => new ResourceReference { Reference = $"#{o.Id}" }).ToList(),
                Subject = new ResourceReference("#")
            };

            if (Issues.Any())
            {
                assessment.Contained.Add(new OperationOutcome
                {
                    Issue = Issues.ToList()
                });
            }

            SetMeta(assessment);

            return assessment;
        }
    }
}
