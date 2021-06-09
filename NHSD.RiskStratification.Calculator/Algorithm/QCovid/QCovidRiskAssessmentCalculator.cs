using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.RiskStratification.Calculator.Coding;
using NHSD.RiskStratification.Calculator.FhirJson;
using QCovid.RiskCalculator.Exceptions;
using QCovid.RiskCalculator.Risk.Result;
using QCovid.RiskCalculator.Townsend;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid
{
    public sealed class QCovidRiskAssessmentCalculator : RiskAssessmentCalculator<Coding.RiskInput>
    {
        private readonly IRiskCalculator<global::QCovid.RiskCalculator.Risk.Input.RiskInput, IRiskResult> _riskCalculator;

        protected internal override AlgorithmAssembly AlgorithmAssembly { get; } = QCovidCalculator.AlgorithmAssembly;

        public QCovidRiskAssessmentCalculator(IRiskCalculator<global::QCovid.RiskCalculator.Risk.Input.RiskInput, IRiskResult> riskCalculator)
        {
            _riskCalculator = riskCalculator ?? throw new ArgumentNullException(nameof(riskCalculator));
        }

        public override RiskAssessmentCalculation CreateAssessment(ObservationParameterExtractionContext<Coding.RiskInput> parameterExtraction)
        {
            var parameters = parameterExtraction.Parameters;
            var observations = parameterExtraction.ParameterObservations;

            var riskResult = _riskCalculator.CalculateRisk(parameters);
            var issues = riskResult.ErrorCodes.Select(MapError);

            if (riskResult.ErrorCodes.Any(x => x.IsFatal))
            {
                return new RiskAssessmentCalculation(AlgorithmAssembly, issues);
            }

            // Only include postcode observations if they increase the risk.
            if (parameterExtraction.ChildContext<PostcodeInfo>() is { } postcodeContext)
            {
                // We determine if the risk changes but recalculating the risk without the Townsend score.
                RiskResultForSubjectType riskWithoutTownsend;

                if (parameters.EncryptedTownsendScore == EncryptedTownsendScore.Default)
                {
                    // No need to recalculate using the same input.
                    riskWithoutTownsend = riskResult.ResultForPatient;
                }
                else
                {
                    var resultWithoutTownsend = _riskCalculator.CalculateRisk(new global::QCovid.RiskCalculator.Risk.Input.RiskInput(parameters.Age, parameters.Bmi, parameters.Sex, EncryptedTownsendScore.Default, parameters.HousingCategory, parameters.Ethnicity, parameters.ClinicalInformation));

                    if (resultWithoutTownsend.ErrorCodes.Any(x => x.IsFatal))
                    {
                        return new RiskAssessmentCalculation(AlgorithmAssembly, resultWithoutTownsend.ErrorCodes.Select(MapError));
                    }

                    riskWithoutTownsend = resultWithoutTownsend.ResultForPatient;
                }

                var increasedRiskOfDeath = riskResult.ResultForPatient.Death.RiskPercentage > riskWithoutTownsend.Death.RiskPercentage;
                var increasedRiskOfHospitalisation = riskResult.ResultForPatient.Hospitalisation.RiskPercentage > riskWithoutTownsend.Hospitalisation.RiskPercentage;

                // If the postcode has not increased the risk, then remove the postcode from the observations.
                if (!increasedRiskOfDeath && !increasedRiskOfHospitalisation)
                {
                    observations = observations.Except(postcodeContext.ParameterObservations).ToList();
                }
            }

            var predictions = RiskPredictions(riskResult);

            return new RiskAssessmentCalculation(AlgorithmAssembly, predictions, observations, issues);
        }

        private IEnumerable<RiskAssessment.PredictionComponent> RiskPredictions(IRiskResult riskResult)
        {
            const string RiskAssessmentSystem = "http://nhsd.nhs.uk/riskassessment";

            return new[] {

                new RiskAssessment.PredictionComponent
                {
                    Outcome = new CodeableConcept( SnomedCodingSystem.System, "419620001", "Death (event)","Death attributed to COVID-19"),
                    Probability = Probability(riskResult.ResultForPatient.Death),
                    RelativeRisk =  RelativeRisk(riskResult.ResultForPatient.Death, riskResult.ResultForTypicalPersonSameAgeSex.Death)
                },
                new RiskAssessment.PredictionComponent
                {
                    Outcome = new CodeableConcept(RiskAssessmentSystem, "Hospitalisation", "Hospitalisation", "COVID-19"),
                    Probability = Probability(riskResult.ResultForPatient.Hospitalisation),
                    RelativeRisk =  RelativeRisk(riskResult.ResultForPatient.Hospitalisation, riskResult.ResultForTypicalPersonSameAgeSex.Hospitalisation)
                },
                new RiskAssessment.PredictionComponent
                {
                    Outcome = new CodeableConcept(RiskAssessmentSystem, "BaselineDeath",  "Baseline death for age and sex", "Death attributed to COVID-19 for same age and sex but no other risk factors"),
                    Probability = Probability(riskResult.ResultForTypicalPersonSameAgeSex.Death),
                    RelativeRisk =  1
                },
                new RiskAssessment.PredictionComponent
                {
                    Outcome = new CodeableConcept(RiskAssessmentSystem, "BaselineHospitalisation", "Baseline hospitalisation for age and sex", "COVID-19 for same age and sex but no other risk factors"),
                    Probability = Probability(riskResult.ResultForTypicalPersonSameAgeSex.Hospitalisation),
                    RelativeRisk = 1
                },
            };
        }

        private static decimal Probability(RiskResultForOutcomeType value)
        {
            return (decimal)value.RiskPercentage;
        }

        private static decimal RelativeRisk(RiskResultForOutcomeType value, RiskResultForOutcomeType baseline)
        {
            return (decimal)value.RiskPercentage / (decimal)baseline.RiskPercentage;
        }

        private static OperationOutcome.IssueComponent MapError(QCovidErrorCode error)
        {
            return new OperationOutcome.IssueComponent
            {
                Severity = error.IsFatal ? OperationOutcome.IssueSeverity.Error : OperationOutcome.IssueSeverity.Warning,
                Code = OperationOutcome.IssueType.Processing,
                Details = new CodeableConcept("http://qcovid.org", error.Code.ToString(), error.Message)
            };
        }
    }
}
