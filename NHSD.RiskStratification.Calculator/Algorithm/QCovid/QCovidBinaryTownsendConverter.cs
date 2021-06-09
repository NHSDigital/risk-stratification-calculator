using QCovid.RiskCalculator.Townsend;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid
{
    public class QCovidBinaryTownsendConverter : QCovidTownsendConverter
    {
        public QCovidBinaryTownsendConverter(string licenceKey, ITownsendScoreDbReader scoreReader)
        {
            if (scoreReader != null)
            {
                Converter = new PostcodeToTownsendScoreConverter(licenceKey, scoreReader);
            }
        }

        protected override PostcodeToTownsendScoreConverter Converter { get; }

        public override void Dispose()
        {
            //no-op
        }
    }
}
