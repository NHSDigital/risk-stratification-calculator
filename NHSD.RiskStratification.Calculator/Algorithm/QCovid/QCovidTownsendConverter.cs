using QCovid.RiskCalculator.Townsend;
using System;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid
{
    public abstract class QCovidTownsendConverter : IDisposable
    {
        protected abstract PostcodeToTownsendScoreConverter Converter { get; }

        public EncryptedTownsendScore GetScore(string postcode)
        {
            if (string.IsNullOrWhiteSpace(postcode))
            {
                return EncryptedTownsendScore.Default;
            }

            return Converter?.GetTownsendScore(new Postcode(postcode)) ?? EncryptedTownsendScore.Default;
        }

        public abstract void Dispose();
    }
}
