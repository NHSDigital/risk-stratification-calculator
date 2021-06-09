#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.RiskStratification.Calculator.FhirJson;

namespace NHSD.RiskStratification.Calculator
{
    public class ObservationParameterExtractionContext<TParameters>
    {
        private TParameters _parameters = default!;
        private readonly ObservationIndex _observationIndex;
        private readonly List<Observation> _parameterObservations = new List<Observation>();
        private readonly List<ParameterExtractionIssue> _issues = new List<ParameterExtractionIssue>();
        private readonly IDictionary<Type, object> _childContexts = new Dictionary<Type, object>();

        public ObservationParameterExtractionContext(ObservationIndex observationIndex)
        {
            _observationIndex = observationIndex;
        }

        public bool Success => _observationIndex.Issues.Count == 0 && _issues.Count == 0;

        public TParameters Parameters => Success ? _parameters : throw new InvalidOperationException($"Parameters cannot be supplied for an invalid input bundle. Please check {nameof(Issues)} for details");

        public IEnumerable<ParameterExtractionIssue> Issues => _observationIndex.Issues.Concat(_issues);

        public IReadOnlyCollection<Observation> ParameterObservations => _parameterObservations;

        public ObservationParameterExtractionContext<T>? ChildContext<T>()
        {
            if (_childContexts.TryGetValue(typeof(T), out var childContext))
            {
                return (ObservationParameterExtractionContext<T>)childContext;
            }

            return null;
        }

        public T Extract<T>(IObservationParameterExtractor<T> parameterExtractor)
        {
            if (_childContexts.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"Already extracted {typeof(T).Name} in this context.");
            }

            var context = new ObservationParameterExtractionContext<T>(this._observationIndex);
            context._parameters = parameterExtractor.Extract(context);

            _parameterObservations.AddRange(context._parameterObservations);
            _issues.AddRange(context._issues);

            _childContexts.Add(typeof(T), context);

            return context._parameters;
        }

        private ObservationParameterExtractionContext<TParameters> ExtractParameters(IObservationParameterExtractor<TParameters> parameterExtractor)
        {
            _parameters = parameterExtractor.Extract(this);
            return this;
        }

        public decimal? RequireQuantityObservation(CodeableConcept observationCode, params (Func<decimal, bool> check, string message)[] validations)
        {
            var search = RequireObservation(observationCode);

            if (TryGetObservationValue<Quantity>(search, out var quantity) && quantity.Value is { } value)
            {
                var invalid = false;

                foreach (var (check, message) in validations)
                {
                    if (check(value))
                    {
                        InvalidIssue(message, observationCode, search.Observation!.Id);
                        invalid = true;
                    }
                }

                return invalid ? (decimal?)null : value;
            }

            return null;
        }

        [return: MaybeNull]
        public TConversion GetConvertedCodedObservation<TConversion>(CodeableConcept observationCode, Func<CodeableConcept, TConversion> conversionFunc)
        {
            return GetConvertedCodedObservation(FindObservation(observationCode), conversionFunc);
        }

        [return: MaybeNull]
        public TConversion RequireConvertedCodedObservation<TConversion>(CodeableConcept observationCode, Func<CodeableConcept, TConversion> conversionFunc)
        {
            return GetConvertedCodedObservation(RequireObservation(observationCode), conversionFunc);
        }

        public string? GetStringObservation(CodeableConcept observationCode)
        {
            return TryGetObservationValue<string>(FindObservation(observationCode), out var value) ? value : null;
        }

        public bool? GetBooleanObservation(CodeableConcept observationCode)
        {
            return GetObservationValue<bool>(FindObservation(observationCode));
        }

        [return: MaybeNull]
        private TConversion GetConvertedCodedObservation<TConversion>(ObservationSearchResult search, Func<CodeableConcept, TConversion> conversionFunc)
        {
            if (GetCodedObservation(search) is { } codedValue)
            {
                var converted = conversionFunc(codedValue);
                if (converted != null)
                {
                    return converted;
                }

                InvalidIssue($"Unable to convert CodeableConcept[{codedValue.Coding.FirstOrDefault().Code}] using system [{codedValue.Coding.FirstOrDefault().System}] to [{typeof(TConversion)}]", search.SearchTerm, search.Observation!.Id!);
            }

            return default;
        }

        private CodeableConcept? GetCodedObservation(ObservationSearchResult search)
        {
            if (TryGetObservationValue<CodeableConcept>(search, out var observationCodedValue))
            {
                if (observationCodedValue.Coding.Any() != true)
                {
                    IncompleteIssue("coded value(s) are required", search.SearchTerm, search.Observation?.Id);
                    return null;
                }

                return observationCodedValue;
            }

            return null;
        }

        public TValue? GetObservationValue<TValue>(ObservationSearchResult search) where TValue : struct
        {
            return TryGetObservationValue<TValue>(search, out var value) ? value : (TValue?)null;
        }

        public bool TryGetObservationValue<TValue>(ObservationSearchResult search, [MaybeNullWhen(false)] out TValue value)
        {
            value = default!;
            var observation = search.Observation;

            if (observation is null)
            {
                return false;
            }

            if (observation.Id is null)
            {
                IncompleteIssue("Id is required", observation.Code);
                return false;
            }

            if (observation.ValueType is { } valueType && typeof(TValue).IsAssignableFrom(valueType) is false)
            {
                InvalidIssue($"Value must be of type [{typeof(TValue).Name}]", observation.Code, observation.Id);
                return false;
            }

            if (observation.TryGetValue<TValue>(out var val))
            {
                value = val;
                _parameterObservations.Add(observation);
                return true;
            }

            IncompleteIssue("Must have a value", observation.Code, observation.Id);
            return false;
        }

        public ObservationSearchResult FindObservation(CodeableConcept observationCode) => _observationIndex.FindObservation(observationCode);

        private ObservationSearchResult RequireObservation(CodeableConcept observationCode)
        {
            var search = FindObservation(observationCode);

            if (search.Observation == null)
            {
                IncompleteIssue("is required", observationCode);
            }

            return search;
        }

        private void IncompleteIssue(string message, CodeableConcept? observationCode, params string?[] observationIds)
        {
            _issues.Add(new ParameterExtractionIssue(message, OperationOutcome.IssueType.Incomplete, observationCode, observationIds));
        }

        private void InvalidIssue(string message, CodeableConcept? observationCode, params string?[] observationIds)
        {
            _issues.Add(new ParameterExtractionIssue(message, OperationOutcome.IssueType.Invalid, observationCode, observationIds));
        }

        public static ObservationParameterExtractionContext<TParameters> FromBundle<TParameterExtractor>(TParameterExtractor parameterExtractor, Bundle observationBundle)
            where TParameterExtractor : IObservationParameterExtractor<TParameters>
        {
            var observations = observationBundle.GetResources().OfType<Observation>();

            var observationIndex = new ObservationIndex(observations);

            return new ObservationParameterExtractionContext<TParameters>(observationIndex).ExtractParameters(parameterExtractor);
        }
    }
}
