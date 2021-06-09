namespace NHSD.RiskStratification.Calculator.Algorithm
{
    public abstract class RiskAssessmentCalculator<TParameters>
    {
        protected internal abstract AlgorithmAssembly AlgorithmAssembly { get; }

        public abstract RiskAssessmentCalculation CreateAssessment(ObservationParameterExtractionContext<TParameters> parameterExtraction);
    }
}
