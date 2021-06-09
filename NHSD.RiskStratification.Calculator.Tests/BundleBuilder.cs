using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator.Tests
{
    public sealed class BundleBuilder
    {
        public BundleBuilder AddResourceEntry(string id, CodeableConcept code, string value)
        {
            return AddResourceEntry(new ObservationString { Id = id, Code = code, Value = value });
        }

        public BundleBuilder AddResourceEntry(string id, CodeableConcept code, CodeableConcept value)
        {
            return AddResourceEntry(new ObservationCodeableConcept { Id = id, Code = code, Value = value });
        }

        public BundleBuilder AddResourceEntry(string id, CodeableConcept code, Quantity value)
        {
            return AddResourceEntry(new ObservationQuantity { Id = id, Code = code, Value = value });
        }

        public BundleBuilder AddResourceEntry(string id, CodeableConcept code, bool value)
        {
            return AddResourceEntry(new ObservationBoolean { Id = id, Code = code, Value = value });
        }

        public BundleBuilder AddResourceEntry(Observation observation)
        {
            Bundle.AddResourceEntry(observation, null);

            return this;
        }

        public BundleBuilder AddResourceEntry(CodeableConcept code, CodeableConcept value) => AddResourceEntry(GenerateId(), code, value);
        public BundleBuilder AddResourceEntry(CodeableConcept code, bool value) => AddResourceEntry(GenerateId(), code, value);

        private string GenerateId() => $"obs{Bundle.Entry.Count}";

        public Bundle Bundle { get; } = new Bundle
        {
            Type = Bundle.BundleType.Transaction
        };
    }
}
