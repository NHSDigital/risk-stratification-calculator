using System;
using QCovid.RiskCalculator.Townsend;

namespace NHSD.RiskStratification.Calculator.Coding
{
    public class PostcodeInfo
    {
        public PostcodeInfo(EncryptedTownsendScore townsendScore, string postcodeArea)
        {
            TownsendScore = townsendScore;
            PostcodeArea = postcodeArea;
        }

        public EncryptedTownsendScore TownsendScore { get; }

        public string PostcodeArea { get; }
    }
}
