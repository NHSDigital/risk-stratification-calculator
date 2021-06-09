#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class Observation : Resource
    {
        public override string ResourceType => nameof(Observation);

        [JsonProperty("code", Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        public CodeableConcept? Code { get; set; }

        [JsonIgnore] public virtual Type? ValueType => null;

        public virtual bool TryGetValue<T>([NotNullWhen(true)] out T value)
        {
            value = default!;
            return false;
        }
    }

    public class ObservationCodeableConcept : Observation
    {
        public override Type ValueType => typeof(CodeableConcept);

        [JsonProperty("valueCodeableConcept", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public CodeableConcept? Value { get; set; }

        public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value)
        {
            if (Value is TValue val)
            {
                value = val;
                return true;
            }

            value = default!;
            return false;
        }
    }

    public class ObservationBoolean : Observation
    {
        public override Type ValueType => typeof(bool);

        [JsonProperty("valueBoolean", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public bool? Value { get; set; }

        public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value)
        {
            if (Value is TValue val)
            {
                value = val;
                return true;
            }

            value = default!;
            return false;
        }
    }

    public class ObservationInteger : Observation
    {
        public override Type ValueType => typeof(int);

        [JsonProperty("valueInteger", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public int? Value { get; set; }

        public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value)
        {
            if (Value is TValue val)
            {
                value = val;
                return true;
            }

            value = default!;
            return false;
        }
    }

    public class ObservationQuantity : Observation
    {
        public override Type ValueType => typeof(Quantity);

        [JsonProperty("valueQuantity", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public Quantity? Value { get; set; }

        public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value)
        {
            if (Value is TValue val)
            {
                value = val;
                return true;
            }

            value = default!;
            return false;
        }
    }

    public class ObservationString : Observation
    {
        private string? _value;
        
        public override Type ValueType => typeof(string);

        [JsonProperty("valueString", Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        public string? Value
        {
            get => _value;
            set => _value = string.IsNullOrWhiteSpace(value) ? null : value;
        }

        public override bool TryGetValue<TValue>([NotNullWhen(true)] out TValue value)
        {
            if (Value is TValue val)
            {
                value = val;
                return true;
            }

            value = default!;
            return false;
        }
    }
}
