using System;
using NHSD.RiskStratification.Calculator.Algorithm;
using NHSD.RiskStratification.Calculator.Algorithm.QCovid;
using NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex;
using NHSD.RiskStratification.Calculator.Coding;
using NHSD.RiskStratification.Calculator.Infrastructure;
using NHSD.RiskStratification.Calculator.Infrastructure.Logging;
using QCovid.RiskCalculator.Risk.Input;
using QCovid.RiskCalculator.Townsend;
using Serilog;

namespace NHSD.RiskStratification.Calculator.Lambda
{
    public class QCovidSnomedLambda : RiskCalculatorLambda<Coding. RiskInput>, IDisposable
    {
        private readonly QCovidObservationParameterExtractor parameterExtractor;

        protected override IObservationParameterExtractor<Coding.RiskInput> ParameterExtractor => parameterExtractor;

        protected override RiskAssessmentCalculator<Coding.RiskInput> RiskCalculator { get; }

        public QCovidSnomedLambda() : this(LogManager.GetLogger(), GetTownsendReader())
        {
        }

        public QCovidSnomedLambda(ILogger logger, ITownsendScoreDbReader townsendIndexReader)
        {
            var licenceKey = ApplicationConfiguration.QCovidSecrets?.QCovidLicenceKey;

            var townsendConverter = new QCovidBinaryTownsendConverter(licenceKey, townsendIndexReader);
            parameterExtractor = new QCovidObservationParameterExtractor(townsendConverter);

            RiskCalculator = new QCovidRiskAssessmentCalculator(new QCovidCalculator(licenceKey));
            Logger = logger.GetLoggerWithAlgorithmContext<QCovidSnomedLambda>(RiskCalculator.AlgorithmAssembly, "qcovid");
        }

        private static ITownsendScoreDbReader GetTownsendReader()
        {
            return string.IsNullOrWhiteSpace(ApplicationConfiguration.TownsendIndex?.Bucket)
                ? null
                : new BucketedIndexReader(new S3TownsendIndexReader(ApplicationConfiguration.TownsendIndex), LogManager.GetLogger());
        }

        public void Dispose()
        {
            parameterExtractor.Dispose();
        }
    }
}
