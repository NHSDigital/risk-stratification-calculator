using QCovid.RiskCalculator.BodyMassIndex;
using QCovid.RiskCalculator.Risk.Input;
using QCovid.RiskCalculator.Townsend;

namespace NHSD.RiskStratification.Calculator.Coding
{
    public class RiskInput : QCovid.RiskCalculator.Risk.Input.RiskInput
    {
        public RiskInput(
            Age age,
            Bmi bmi,
            Sex sex,
            EncryptedTownsendScore townsendScore,
            HousingCategory housingCategory,
            Ethnicity ethnicity,
            ClinicalInformation clinicalInformation) : base(
            age,
            bmi,
            sex,
            townsendScore,
            housingCategory,
            ethnicity,
            clinicalInformation)
        {
        }

        public string PostcodeArea { get; set; }
    }
}
