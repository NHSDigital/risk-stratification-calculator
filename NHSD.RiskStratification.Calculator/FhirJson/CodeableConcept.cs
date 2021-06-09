#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public readonly struct CodeableConcept : IEquatable<CodeableConcept>
    {
        private static readonly IDictionary<CodeableConcept, string> _knownConcepts = new Dictionary<CodeableConcept, string>();
        private readonly IReadOnlyCollection<Coding>? _coding;

        public CodeableConcept(string text) : this(Array.Empty<Coding>(), text)
        {
        }

        public CodeableConcept(string system, string code, string? text = null) : this(new Coding(system, code), text)
        {
        }

        public CodeableConcept(string system, string code, string display, string text) : this(new Coding(system, code, display), text)
        {
        }

        public CodeableConcept(Coding coding, string? text = null) : this(new[] { coding }, text)
        {
        }

        [JsonConstructor]
        public CodeableConcept(IEnumerable<Coding>? coding = null, string? text = null)
        {
            _coding = coding?.ToArray();

            if (_coding?.Count == 0)
            {
                _coding = null;
            }

            Text = !string.IsNullOrWhiteSpace(text) ? text : null;
        }

        [JsonProperty("coding", NullValueHandling = NullValueHandling.Ignore)]
        public IReadOnlyCollection<Coding> Coding => _coding ?? Array.Empty<Coding>();

        /// <summary>
        /// Convention used by <see cref="Newtonsoft"/> to control serialisation of <see cref="Coding"/>
        /// </summary>
        public bool ShouldSerializeCoding() => Coding.Count > 0;

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string? Text { get; }

        public override string ToString()
        {
            var txt = new StringBuilder();

            if (_knownConcepts.TryGetValue(this, out var name))
            {
                txt.Append(name);
            }

            if (Text != null)
            {
                if (txt.Length > 0)
                {
                    txt.Append(" ");
                }

                txt.Append(Text);
            }

            if (Coding.Count > 0)
            {
                if (txt.Length > 0)
                {
                    txt.Append(" ");
                }

                foreach (var coding in Coding)
                {
                    txt.Append(", ").Append(coding);
                }
            }

            return txt.ToString();
        }

        public bool Equals(CodeableConcept other)
        {
            if (!Coding.SequenceEqual(other.Coding))
            {
                return false;
            }

            return Text == other.Text;
        }

        public override bool Equals(object? obj)
        {
            return obj is CodeableConcept other && Equals(other);
        }

        public override int GetHashCode() => HashCode.Combine(Coding, Text);

        public static void RegisterConceptName(CodeableConcept concept, string name) => _knownConcepts.Add(concept, name);
    }
}
