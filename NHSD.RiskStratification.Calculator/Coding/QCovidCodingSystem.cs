using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator.Coding
{
    public static class QCovidCodingSystem
    {
        public const string System = "http://nhsd.riskstrat.nhs/qcovid";

        private static CodeableConcept Register(string code, string name)
        {
            var concept = new CodeableConcept(System, code);
            CodeableConcept.RegisterConceptName(concept, name);
            return concept;
        }

        public static CodeableConcept None { get; } = Register("NONE", "None");

        public static CodeableConcept CoronaryHeartDisease { get; } = Register("CORONARY_HEART_DISEASE", "Coronary heart disease");

        public static CodeableConcept HeartFailure { get; } = Register("HEART_FAILURE", "Heart failure");

        public static CodeableConcept CongenitalHeartDisease { get; } = Register("CONGENITAL_HEART_DISEASE", "Congenital heart disease or has had surgery for it in the past");

        public static CodeableConcept PeripheralVascularDisease { get; } = Register("PERIPHERAL_VASCULAR_DISEASE", "Peripheral vascular disease");

        public static CodeableConcept Asthma { get; } = Register("ASTHMA", "Has asthma");

        public static CodeableConcept ChronicObstructivePulmonaryDisease { get; } = Register("COPD", "Has chronic obstructive pulmonary disease");
        public static CodeableConcept CysticFibrosis { get; } = Register("CYSTIC_FIBROSIS", "Cystic fibrosis, bronchiectasis or alveolitis");
        public static CodeableConcept Hypertension { get; } = Register("HYPERTENSION", "Pulmonary hypertension or pulmonary fibrosis");
        public static CodeableConcept DiabetesType { get; } = Register("DIABETES_TYPE", "Diabetes type");
        public static CodeableConcept DiabetesType1 { get; } = Register("TYPE_1_DIABETES", "Type 1 Diabetes");
        public static CodeableConcept DiabetesType2 { get; } = Register("TYPE_2_DIABETES", "Type 2 Diabetes");
        public static CodeableConcept Fracture { get; } = Register("FRACTURE", "Osteoporotic fracture of hip, wrist, spine or humerus");

        public static CodeableConcept KidneyDisease { get; } = Register("KIDNEY_DISEASE", "Kidney disease");
        public static CodeableConcept CKD3 { get; } = new CodeableConcept(System, "CKD3");
        public static CodeableConcept CKD4 { get; } = new CodeableConcept(System, "CKD4");
        public static CodeableConcept CKD5WithoutDialysis { get; } = Register("CKD5_NO_DIALYSIS", "CKD5 without dialysis or transplant");
        public static CodeableConcept CKD5WithDialysis { get; } = Register("CKD5_DIALYSIS", "CKD with dialysis in last 12 months");
        public static CodeableConcept CKD5WithTransplant { get; } = Register("CKD_TRANSPLANT", "CKD with transplant");

        public static CodeableConcept Epilepsy { get; } = Register("EPILEPSY", "Epilepsy");
        public static CodeableConcept StrokeOrTIA { get; } = Register("STROKE_TIA", "Stroke or TIA");
        public static CodeableConcept MotorNeuroneDisease { get; } = Register("MND", "Motor neurone disease, multiple sclerosis, myaesthenia, or Huntingtons's Chorea");

        public static CodeableConcept LearningDisability { get; } = Register("LEARNING_DISABILITY", "Learning disability excluding Down's Syndrome");
        public static CodeableConcept DownsSyndrome { get; } = Register("DOWNS_SYNDROME", "Down's Syndrome");
        public static CodeableConcept SevereMentalIllness { get; } = Register("SEVERE_MENTAL_ILLNESS", "Severe mental illness");
        public static CodeableConcept RheumatoidArthritis { get; } = Register("RHEUMATOID_ARTHRITIS", "Rheumatoid arthritis or SLE");
        public static CodeableConcept SickleCellDisease { get; } = Register("SICKLE_CELL_DISEASE", "Sickle cell disease or severe combined immune deficiency syndromes");
        public static CodeableConcept Thrombosis { get; } = Register("THROMBOSIS", "Thrombosis or pulmonary embolus");
        public static CodeableConcept LungOrOralCancer { get; } = Register("LUNG_ORAL_CANCER", "Lung or oral cancer");
        public static CodeableConcept OrganTransplant { get; } = Register("ORGAN_TRANSPLANT", "Solid organ transplant (lung, liver, stomach, pancreas, spleen, heart or thymus)");
        public static CodeableConcept BloodCancer { get; } = Register("BLOOD_CANCER", "Cancer of the blood or bone marrow such as leukaemia, myelodysplastic syndromes, lymphoma or myeloma and is at any stage of treatment");
        public static CodeableConcept BoneMarrowTransplant { get; } = Register("BONE_MARROW_TRANSPLANT", "Bone marrow or stem cell transplant in the last 6 months");
        public static CodeableConcept Radiotherapy { get; } = Register("RADIOTHERAPY", "Radiotherapy in the last 6 months");

        public static CodeableConcept AntiLeukotrieneOrLaba { get; } = Register("ANTI_LEUKOTRIENE", "Is taking anti-leukotriene or long acting beta2-agonists");

        public static CodeableConcept Chemotherapy { get; } = Register("CHEMOTHERAPY", "Chemotherapy in the last 12 months");

        public static CodeableConcept ChemotherapyGroupA { get; } = Register("SACT_CHEMOTHERAPY_A", "Chemotherapy systemic anti cancer therapy (SACT) protocol A");

        public static CodeableConcept ChemotherapyGroupB { get; } = Register("SACT_CHEMOTHERAPY_B", "Chemotherapy systemic anti cancer therapy (SACT) protocol B");

        public static CodeableConcept ChemotherapyGroupC { get; } = Register("SACT_CHEMOTHERAPY_C", "Chemotherapy systemic anti cancer therapy (SACT) protocol C");

        public static CodeableConcept OralPrednisolone { get; } = Register("ORAL_PREDNISOLONE", "Is taking oral prednisolone");

        public static CodeableConcept PrescribedImmunosuppressants { get; } = Register("IMMUNOSUPPRESSANTS", "Has been prescribed immunosuppressants");
    }
}
