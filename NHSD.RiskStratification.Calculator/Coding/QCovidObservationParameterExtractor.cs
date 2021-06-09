using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NHSD.RiskStratification.Calculator.Algorithm.QCovid;
using NHSD.RiskStratification.Calculator.FhirJson;
using QCovid.RiskCalculator.BodyMassIndex;
using QCovid.RiskCalculator.Risk.Input;

namespace NHSD.RiskStratification.Calculator.Coding
{
    public class QCovidObservationParameterExtractor : IObservationParameterExtractor<RiskInput>, IObservationParameterExtractor<PostcodeInfo>, IDisposable
    {
        private readonly QCovidTownsendConverter _townsendConverter;

        public QCovidObservationParameterExtractor(QCovidTownsendConverter townsendConverter)
        {
            _townsendConverter = townsendConverter;
        }

        public RiskInput Extract(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var age = ExtractAge(extractionContext);
            var bmi = ExtractBmi(extractionContext);
            var sex = extractionContext.RequireConvertedCodedObservation(SnomedCodingSystem.Sex, ConvertSex);
            var ethnicGroup = extractionContext.GetConvertedCodedObservation(SnomedCodingSystem.EthnicGroup, ConvertEthnicGroup) ?? Ethnicity.NotRecorded;
            var housing = extractionContext.GetConvertedCodedObservation(SnomedCodingSystem.Housing, ConvertHousing) ?? HousingCategory.NeitherInNursingOrCareHomeNorHomeless;
            var postcodeInfo = extractionContext.Extract<PostcodeInfo>(this);

            ClinicalInformation clinical = ExtractClinicalInformation(extractionContext);

            return new RiskInput(
                age,
                bmi,
                sex,
                postcodeInfo?.TownsendScore,
                housing,
                ethnicGroup,
                clinical
            )
            {
                PostcodeArea = postcodeInfo?.PostcodeArea
            };
        }

        private Age ExtractAge(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var age = (int?)extractionContext.RequireQuantityObservation(
                    SnomedCodingSystem.Age,
                    (age => decimal.Truncate(age) != age, "Value must be an integer"),
                    (age => !QCovidValidation.ValidAge((int)age), $"Value must be between {QCovidValidation.MinimumAgeInclusive} and {QCovidValidation.MaximumAgeInclusive}"));

            return age.HasValue ? new Age(age.Value) : null;
        }

        private Bmi ExtractBmi(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var heightSearch = extractionContext.FindObservation(SnomedCodingSystem.Height);
            var weightSearch = extractionContext.FindObservation(SnomedCodingSystem.Weight);

            // Only try and get values if both observations are present as getting values causes side effects of parameter observations.
            if (!heightSearch.HasObservation || !weightSearch.HasObservation)
            {
                return Bmi.Default;
            }

            if (extractionContext.TryGetObservationValue<Quantity>(heightSearch, out var heightQuantity)
                && heightQuantity is { Unit: "cm", Value: { } height }
                && extractionContext.TryGetObservationValue<Quantity>(weightSearch, out var weightQuantity)
                && weightQuantity is { Unit: "kg", Value: { } weight })
            {
                return Bmi.CreateFromKgAndCm((double)weight, (double)height);
            }

            return Bmi.Default;
        }

        public PostcodeInfo Extract(ObservationParameterExtractionContext<PostcodeInfo> extractionContext)
        {
            static string ExtractPostcodeArea(string s)
            {
                if (Postcode.TryParse(s, out var postcode))
                {
                    return postcode.Area;
                }

                return null;
            }

            var postcode = extractionContext.GetStringObservation(SnomedCodingSystem.PostCode);

            var score = _townsendConverter.GetScore(postcode);
            var area = ExtractPostcodeArea(postcode);

            return new PostcodeInfo(score, area);
        }

        private ClinicalInformation ExtractClinicalInformation(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var chronicKidneyDisease = extractionContext
                                           .GetConvertedCodedObservation(QCovidCodingSystem.KidneyDisease, ConvertKidneyDisease) ?? ChronicKidneyDisease.None;

            var diabetes = extractionContext
                .GetConvertedCodedObservation(QCovidCodingSystem.DiabetesType, ConvertDiabetes) ?? Diabetes.None;

            var sickleCellDiseaseOrCombinedImmuneDeficiency = extractionContext.GetBooleanObservation(QCovidCodingSystem.SickleCellDisease).GetValueOrDefault(false)
                ? SickleCellOrSevereCombinedImmunodeficiencySyndrome.Present
                : SickleCellOrSevereCombinedImmunodeficiencySyndrome.None;

            var learningDisabilityOrDownsSyndrome = extractionContext
                .GetConvertedCodedObservation(SnomedCodingSystem.LearningDisabilityOrDownsSyndrome, ConvertLearningDisability) ?? LearningDisabilityOrDownsSyndrome.Neither;

            var cancerAndImmunSup = ExtractCancerTreatmentsAndImmunoSuppressants(extractionContext);
            var heartOrCirc = ExtractHeartOrCirculationProblems(extractionContext);
            var neurological = ExtractNeurologicalProblems(extractionContext);
            var respiratory = ExtractRespiratoryProblems(extractionContext);
            var other = ExtractOtherConditions(extractionContext);

            return new ClinicalInformation(
                diabetes,
                chronicKidneyDisease,
                sickleCellDiseaseOrCombinedImmuneDeficiency,
                learningDisabilityOrDownsSyndrome,
                cancerAndImmunSup,
                respiratory,
                heartOrCirc,
                neurological,
                other);
        }

        private CancerTreatmentsAndImmunoSuppressants ExtractCancerTreatmentsAndImmunoSuppressants(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var boneMarrowStemCellTransplant6Months = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.BoneMarrowTransplant) ?? false;

            var cancerOfBloodOrBoneMarrow = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.BloodCancer) ?? false;

            var chemotherapy12Months = extractionContext
                .GetConvertedCodedObservation(QCovidCodingSystem.Chemotherapy, ConvertChemotherapy) ?? ChemotherapyGroup.NoneInLast12Months;

            var prescribedImmunosuppressants = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.PrescribedImmunosuppressants) ?? false;

            var prescribedOralSteroids6Months = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.OralPrednisolone) ?? false;

            var radiotherapy6Months = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.Radiotherapy) ?? false;

            var solidOrganTransplant = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.OrganTransplant) ?? false;

            return new CancerTreatmentsAndImmunoSuppressants(
                    chemotherapy12Months,
                    radiotherapy6Months,
                    cancerOfBloodOrBoneMarrow,
                    boneMarrowStemCellTransplant6Months,
                    solidOrganTransplant,
                    prescribedImmunosuppressants,
                    prescribedOralSteroids6Months);
        }

        private HeartOrCirculationProblems ExtractHeartOrCirculationProblems(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var atrialFibrillation = extractionContext.GetBooleanObservation(SnomedCodingSystem.AtrialFibrillation) ?? false;
            var congenitalHeartDisease = extractionContext.GetBooleanObservation(QCovidCodingSystem.CongenitalHeartDisease) ?? false;
            var coronaryHeartDisease = extractionContext.GetBooleanObservation(QCovidCodingSystem.CoronaryHeartDisease) ?? false;
            var heartFailure = extractionContext.GetBooleanObservation(QCovidCodingSystem.HeartFailure) ?? false;
            var peripheralVascularDisease = extractionContext.GetBooleanObservation(QCovidCodingSystem.PeripheralVascularDisease) ?? false;
            var strokeOrTIA = extractionContext.GetBooleanObservation(QCovidCodingSystem.StrokeOrTIA) ?? false;
            var thrombosisOrPulmonaryEmbolus = extractionContext.GetBooleanObservation(QCovidCodingSystem.Thrombosis) ?? false;

            return new HeartOrCirculationProblems(
                congenitalHeartDisease,
                coronaryHeartDisease,
                strokeOrTIA,
                atrialFibrillation,
                heartFailure,
                peripheralVascularDisease,
                thrombosisOrPulmonaryEmbolus);
        }

        private NeurologicalProblems ExtractNeurologicalProblems(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var cerebralPalsy = extractionContext.GetBooleanObservation(SnomedCodingSystem.CerebralPalsy) ?? false;
            var dementia = extractionContext.GetBooleanObservation(SnomedCodingSystem.Dementia) ?? false;
            var epilepsy = extractionContext.GetBooleanObservation(QCovidCodingSystem.Epilepsy) ?? false;
            var motorNeuronDiseaseMultipleSclerosisMyasetheniaHuntingtonsChorea = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.MotorNeuroneDisease) ?? false;
            var parkinsons = extractionContext.GetBooleanObservation(SnomedCodingSystem.Parkinsons) ?? false;

            return new NeurologicalProblems(
                parkinsons,
                epilepsy,
                dementia,
                motorNeuronDiseaseMultipleSclerosisMyasetheniaHuntingtonsChorea,
                cerebralPalsy);
        }

        private RespiratoryProblems ExtractRespiratoryProblems(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var asthma = extractionContext.GetBooleanObservation(QCovidCodingSystem.Asthma) ?? false;

            var chronicObstructivePulmonaryDisease = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.ChronicObstructivePulmonaryDisease) ?? false;

            var cysticFibrosisBronchiectasisAlveolitis = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.CysticFibrosis) ?? false;

            var antiLeukotrieneOrLaba = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.AntiLeukotrieneOrLaba) ?? false;

            var lungOrOralCancer = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.LungOrOralCancer) ?? false;

            var pulmonaryHypertensionOrPulmonaryFibrosis = extractionContext
                .GetBooleanObservation(QCovidCodingSystem.Hypertension) ?? false;

            return new RespiratoryProblems(
                asthma,
                antiLeukotrieneOrLaba,
                cysticFibrosisBronchiectasisAlveolitis,
                pulmonaryHypertensionOrPulmonaryFibrosis,
                chronicObstructivePulmonaryDisease,
                lungOrOralCancer);
        }

        private OtherConditions ExtractOtherConditions(ObservationParameterExtractionContext<RiskInput> extractionContext)
        {
            var cirrhosisOfLiver = extractionContext.GetBooleanObservation(SnomedCodingSystem.CirrhosisAndChronicLiverDisease) ?? false;
            var rheumatoidArthritisOrSLE = extractionContext.GetBooleanObservation(QCovidCodingSystem.RheumatoidArthritis) ?? false;
            var fracture = extractionContext.GetBooleanObservation(QCovidCodingSystem.Fracture) ?? false;
            var severeMentalIllness = extractionContext.GetBooleanObservation(QCovidCodingSystem.SevereMentalIllness) ?? false;

            return new OtherConditions(
                severeMentalIllness,
                cirrhosisOfLiver,
                rheumatoidArthritisOrSLE,
                fracture);
        }

        private readonly (CodeableConcept coding, Sex sex)[] _sexMapping =
        {
            (SnomedCodingSystem.Sex_Male, Sex.Male),
            (SnomedCodingSystem.Sex_Female, Sex.Female)
        };

        private readonly (CodeableConcept coding, ChemotherapyGroup chemotherapyGroup)[] _chemotherapyMapping =
        {
            (QCovidCodingSystem.ChemotherapyGroupA, ChemotherapyGroup.GroupA),
            (QCovidCodingSystem.ChemotherapyGroupB, ChemotherapyGroup.GroupB),
            (QCovidCodingSystem.ChemotherapyGroupC, ChemotherapyGroup.GroupC),
            (QCovidCodingSystem.None, ChemotherapyGroup.NoneInLast12Months),
        };

        private readonly (CodeableConcept coding, Diabetes diabetes)[] _diabetesMappings =
        {
            (QCovidCodingSystem.None, Diabetes.None),
            (QCovidCodingSystem.DiabetesType1, Diabetes.Type1),
            (QCovidCodingSystem.DiabetesType2, Diabetes.Type2)
        };

        private readonly (CodeableConcept coding, HousingCategory housingCategory)[] _housingMappings =
        {
            (SnomedCodingSystem.Housing_Homeless, HousingCategory.Homeless),
            (SnomedCodingSystem.Housing_Independenent, HousingCategory.NeitherInNursingOrCareHomeNorHomeless),
            (SnomedCodingSystem.Housing_CareHome, HousingCategory.NursingOrCareHome)
        };

        private readonly (CodeableConcept coding, LearningDisabilityOrDownsSyndrome learningDisabilityOrDownsSyndrome)[] _learningDisabilityOrDownsSyndromeMappings =
        {
            (QCovidCodingSystem.None, LearningDisabilityOrDownsSyndrome.Neither),
            (QCovidCodingSystem.DownsSyndrome, LearningDisabilityOrDownsSyndrome.DownsSyndrome),
            (QCovidCodingSystem.LearningDisability, LearningDisabilityOrDownsSyndrome.LearningDisabilityExcludingDowns)
        };

        private readonly (CodeableConcept coding, ChronicKidneyDisease chronicKidneyDisease)[] _chronicKidneyDiseaseMappings =
        {
            (QCovidCodingSystem.None, ChronicKidneyDisease.None),
            (QCovidCodingSystem.CKD3, ChronicKidneyDisease.Ckd3),
            (QCovidCodingSystem.CKD4, ChronicKidneyDisease.Ckd4),
            (QCovidCodingSystem.CKD5WithoutDialysis, ChronicKidneyDisease.Ckd5WithNeitherDialysisNorTransplant),
            (QCovidCodingSystem.CKD5WithDialysis, ChronicKidneyDisease.Ckd5WithDialysis),
            (QCovidCodingSystem.CKD5WithTransplant, ChronicKidneyDisease.Ckd5WithTransplant)
        };

        private readonly (CodeableConcept coding, Ethnicity sex)[] _ethnicGroupMapping =
        {
            (SnomedCodingSystem.EthnicGroup_WhiteBritish, Ethnicity.WhiteBritish),
            (SnomedCodingSystem.EthnicGroup_WhiteIrish, Ethnicity.WhiteIrish),
            (SnomedCodingSystem.EthnicGroup_WhiteOther, Ethnicity.OtherWhiteBackground),
            (SnomedCodingSystem.EthnicGroup_MixedBlackCaribbeanAndWhite, Ethnicity.WhiteAndBlackCaribbeanMixed),
            (SnomedCodingSystem.EthnicGroup_MixedBlackAfricanAndWhite, Ethnicity.WhiteAndBlackAfricanMixed),
            (SnomedCodingSystem.EthnicGroup_MixedAsianAndWhite, Ethnicity.WhiteAndAsianMixed),
            (SnomedCodingSystem.EthnicGroup_MixedOtherBackground, Ethnicity.OtherMixedOrMultiEthnic),
            (SnomedCodingSystem.EthnicGroup_AsianIndian, Ethnicity.Indian),
            (SnomedCodingSystem.EthnicGroup_AsianPakistani, Ethnicity.Pakistani),
            (SnomedCodingSystem.EthnicGroup_AsianBangladeshi, Ethnicity.Bangladeshi),
            (SnomedCodingSystem.EthnicGroup_AsianOther, Ethnicity.OtherAsian),
            (SnomedCodingSystem.EthnicGroup_BlackCaribbean, Ethnicity.Caribbean),
            (SnomedCodingSystem.EthnicGroup_BlackAfrican, Ethnicity.BlackAfrican),
            (SnomedCodingSystem.EthnicGroup_BlackOtherBackground, Ethnicity.OtherBlack),
            (SnomedCodingSystem.EthnicGroup_AsianChinese, Ethnicity.Chinese),
            (SnomedCodingSystem.EthnicGroup_AnyOtherGroup, Ethnicity.OtherEthnicGroupIncludingArab),
            (SnomedCodingSystem.EthnicGroup_NotStated, Ethnicity.NotStated)
        };

        private Sex ConvertSex(CodeableConcept sexCode) => GetMapped(_sexMapping, sexCode);

        private Ethnicity ConvertEthnicGroup(CodeableConcept ethnicCode) => GetMapped(_ethnicGroupMapping, ethnicCode);

        private HousingCategory ConvertHousing(CodeableConcept housing) => GetMapped(_housingMappings, housing);

        private ChemotherapyGroup ConvertChemotherapy(CodeableConcept chemo) => GetMapped(_chemotherapyMapping, chemo);

        private Diabetes ConvertDiabetes(CodeableConcept diabetes) => GetMapped(_diabetesMappings, diabetes);

        private LearningDisabilityOrDownsSyndrome ConvertLearningDisability(CodeableConcept learningDisabilities)
            => GetMapped(_learningDisabilityOrDownsSyndromeMappings, learningDisabilities);

        private ChronicKidneyDisease ConvertKidneyDisease(CodeableConcept kidneyDisease)
            => GetMapped(_chronicKidneyDiseaseMappings, kidneyDisease);

        private static TConversion GetMapped<TConversion>(IList<(CodeableConcept coding, TConversion mapped)> mapping, CodeableConcept concept)
        {
            return mapping.SingleOrDefault(entry => concept.Matches(entry.coding)).mapped;
        }

        public void Dispose()
        {
            _townsendConverter.Dispose();
        }
    }
}
