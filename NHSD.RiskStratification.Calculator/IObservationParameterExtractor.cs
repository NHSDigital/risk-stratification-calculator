namespace NHSD.RiskStratification.Calculator
{
    public interface IObservationParameterExtractor<TParameters>
    {
        public TParameters Extract(ObservationParameterExtractionContext<TParameters> extractionContext);
    }
}
