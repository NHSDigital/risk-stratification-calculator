using QCovid.RiskCalculator.Risk.Validation;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid
{
    public static class QCovidValidation
    {
        private static readonly RiskInputValidation _validation = new RiskInputValidation();

        public static readonly int MinimumAgeInclusive = _validation.GetValidAgeRangeYears().MinimumInclusive;

        public static readonly int MaximumAgeInclusive = _validation.GetValidAgeRangeYears().MaximumInclusive;

        public static bool ValidAge(int age) => age >= MinimumAgeInclusive && age <= MaximumAgeInclusive;
    }
}
