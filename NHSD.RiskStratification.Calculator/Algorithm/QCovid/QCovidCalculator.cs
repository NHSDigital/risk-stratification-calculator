using QCovid.RiskCalculator.Risk;
using QCovid.RiskCalculator.Risk.Input;
using QCovid.RiskCalculator.Risk.Result;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid
{
    public sealed class QCovidCalculator : IRiskCalculator<RiskInput, IRiskResult>
    {
        private readonly QCovidRiskCalculator _calculator;

        public static AlgorithmAssembly AlgorithmAssembly { get; } = new AlgorithmAssembly(typeof(QCovidRiskCalculator));

        public QCovidCalculator(string licenceKey)
        {
            _calculator = new QCovidRiskCalculator(licenceKey);
        }

        public IRiskResult CalculateRisk(RiskInput parameters)
        {
            return _calculator.Calculate(parameters);
        }
    }
}
