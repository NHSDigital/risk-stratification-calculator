using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator.Coding
{
    public static class SnomedCodingSystem
    {
        public const string System = "http://snomed.info/sct";

        private static CodeableConcept Register(string code, string name)
        {
            var concept = new CodeableConcept(System, code);
            CodeableConcept.RegisterConceptName(concept, name);

            return concept;
        }

        // Risk Assessment
        public static CodeableConcept LowCovid19ComplicationsRisk { get; } = Register("1300591000000101", "Low risk category for developing complication from COVID-19 infection");
        public static CodeableConcept ModerateCovid19ComplicationsRisk { get; } = Register("1300571000000100", "Moderate risk category for developing complication from COVID-19 infection");
        public static CodeableConcept HighCovid19ComplicationsRisk { get; } = Register("1300561000000107", "High risk category for developing complication from COVID-19 infection");

        // Physical characteristics
        public static CodeableConcept Age { get; } = Register("424144002", "Current chronological age");
        public static CodeableConcept Sex { get; } = Register("734000001", "Biological sex");
        public static CodeableConcept Sex_Female { get; } = Register("248152002", "Female");
        public static CodeableConcept Sex_Male { get; } = Register("248153007", "Male");
        public static CodeableConcept Height { get; } = Register("50373000", "Body height measure");
        public static CodeableConcept Weight { get; } = Register("27113001", "Body weight");
        public static CodeableConcept PostCode { get; } = Register("184102003", "Patient postal code");

        // Ethnicity
        public static CodeableConcept EthnicGroup { get; } = Register("364699009", "Ethnic group");
        public static CodeableConcept EthnicGroup_NotStated { get; } = Register("92531000000104", "Ethnic category not stated");
        public static CodeableConcept EthnicGroup_AnyOtherGroup { get; } = Register("94151000000105", "Any other group");

        // Asian Ethnicity
        public static CodeableConcept EthnicGroup_AsianBangladeshi { get; } = Register("186003008", "Bangladeshi");
        public static CodeableConcept EthnicGroup_AsianChinese { get; } = Register("33897005", "Chinese");
        public static CodeableConcept EthnicGroup_AsianIndian { get; } = Register("110751000000108", "Indian or British Indian");
        public static CodeableConcept EthnicGroup_AsianPakistani { get; } = Register("92461000000105", "Pakistani or British Pakistani");
        public static CodeableConcept EthnicGroup_AsianOther { get; } = Register("92701000000102", "Other Asian or Asian unspecified");

        // Black, African, Black Briish or Carribean
        public static CodeableConcept EthnicGroup_BlackAfrican { get; } = Register("92491000000104", "African");
        public static CodeableConcept EthnicGroup_BlackCaribbean { get; } = Register("107691000000105", "Carribean");
        public static CodeableConcept EthnicGroup_BlackOtherBackground { get; } = Register("92501000000105", "Other Black background");

        // Mixed or Multiple ethnic groups:
        public static CodeableConcept EthnicGroup_MixedAsianAndWhite { get; } = Register("92441000000109", "White and Asian");
        public static CodeableConcept EthnicGroup_MixedBlackAfricanAndWhite { get; } = Register("92431000000100", "White and Black African - ethnic category 2001 census");
        public static CodeableConcept EthnicGroup_MixedBlackCaribbeanAndWhite { get; } = Register("92421000000102", "White and Black Carribean");
        public static CodeableConcept EthnicGroup_MixedOtherBackground { get; } = Register("92451000000107", "Other Mixed background");

        // White
        public static CodeableConcept EthnicGroup_WhiteBritish { get; } = Register("494131000000105", "White British");
        public static CodeableConcept EthnicGroup_WhiteIrish { get; } = Register("494161000000100", "White Irish");
        public static CodeableConcept EthnicGroup_WhiteOther { get; } = Register("92411000000108", "Other White background");

        // Residency
        public static CodeableConcept Housing { get; } = Register("724741000000100", "Housing");
        public static CodeableConcept Housing_Independenent { get; } = Register("160724009", "Independent housing, not alone");
        public static CodeableConcept Housing_Homeless { get; } = Register("266935003", "Housing lack");
        public static CodeableConcept Housing_CareHome { get; } = Register("248171000000108", "Lives in care home");

        // Medical status
        public static CodeableConcept Finding_None { get; } = Register("260413007", "None"); // Generic Not Present status
        public static CodeableConcept Angina { get; } = Register("49601007", "Disorder of cardiovascular system");
        public static CodeableConcept Asthma { get; } = Register("195967001", "Asthma");
        public static CodeableConcept AtrialFibrillation { get; } = Register("49436004", "Atrial fibrillation");
        public static CodeableConcept CancerOfBloodOrBoneMarrow { get; } = Register("94217008", "Secondary malignant neoplasm of bone marrow");
        public static CodeableConcept CerebralPalsy { get; } = Register("128188000", "Cerebral palsy");

        public static CodeableConcept CirrhosisAndChronicLiverDisease { get; } = Register("197279005", "Cirrhosis and chronic liver disease");

        public static CodeableConcept ChronicObstructivePulmonaryDisease { get; } = Register("13645005", "Chronic obstructive lung disease");
        public static CodeableConcept CongenitalHeartDisease { get; } = Register("13213009", "Congenital heart disease");
        public static CodeableConcept CoronaryHeartDisease { get; } = Register("53741008", "Coronary arteriosclerosis");
        public static CodeableConcept Dementia { get; } = Register("52448006", "Dementia");
        public static CodeableConcept Diabetes { get; } = Register("405751000", "Diabetes type");
        public static CodeableConcept Diabetes_Type1 { get; } = Register("46635009", "Diabetes mellitus type 1");
        public static CodeableConcept Diabetes_Type2 { get; } = Register("44054006", "Diabetes mellitus type 2");
        public static CodeableConcept Diabetes_Complication { get; } = Register("165681007", "Hemoglobin A1c greater than 10% indicating poor diabetic control");
        public static CodeableConcept Obese { get; } = Register("443381000124105", "Obese class II");

        public static CodeableConcept HeartFailure { get; } = Register("84114007", "Heart failure");
        public static CodeableConcept LearningDisabilityOrDownsSyndrome { get; } = Register("110359009", "Intellectual disability");

        public static CodeableConcept MalignantNeoplasmActive { get; } = Register("363346000", "Malignant neoplastic disease");
        public static CodeableConcept MalignantNeoplasmRemission { get; } = Register("277022003", "Remission phase");

        public static CodeableConcept ChronicPulmonaryDisease { get; } = Register("87837008", "Chronic pulmonary heart disease");
        public static CodeableConcept OralSteroidsStarted { get; } = Register("170622002", "Oral steroids started");
        public static CodeableConcept Parkinsons { get; } = Register("49049000", "Parkinson's disease");
        public static CodeableConcept PrescribedImmunosuppressants { get; } = Register("372823004", "Immunosuppressant");

        public static CodeableConcept Renal_CKD4 { get; } = Register("431857002", "Chronic kidney disease stage 4");

        public static CodeableConcept Rheumatism { get; } = Register("396332003", "Rheumatism");
        public static CodeableConcept SevereMentalIllness { get; } = Register("391193001", "On severe mental illness register");

        public static CodeableConcept Stroke { get; } = Register("230690007", "Cerebrovascular accident");
    }
}
