#nullable enable
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public class OperationOutcome : Resource
    {
        private List<IssueComponent>? _issue;

        [JsonProperty("issue", Order = 1)]
        public List<IssueComponent> Issue
        {
            get => _issue ??= new List<IssueComponent>();
            set => _issue = value;
        }

        /// <summary>
        /// Convention used by <see cref="Newtonsoft"/> to control serialisation of <see cref="Issue"/>
        /// </summary>
        public bool ShouldSerializeIssue() => _issue?.Count > 0;

        [JsonIgnore]
        public bool Success
        {
            get
            {
                var issues = Issue;

                if (_issue is null)
                {
                    return true;
                }

                return issues.All(i => i.Success);
            }
        }

        public class IssueComponent
        {
            private string? _diagnostics;
            private string[]? _expression;

            [JsonProperty("severity", NullValueHandling = NullValueHandling.Ignore)]
            public IssueSeverity? Severity { get; set; }

            [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
            public IssueType? Code { get; set; }

            [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
            public CodeableConcept? Details { get; set; }

            [JsonProperty("diagnostics", NullValueHandling = NullValueHandling.Ignore)]
            public string? Diagnostics
            {
                get => _diagnostics;
                set => _diagnostics = string.IsNullOrWhiteSpace(value) ? null : value;
            }

            [JsonProperty("expression", NullValueHandling = NullValueHandling.Ignore)]
            public string[]? Expression
            {
                get => _expression;
                set
                {
                    _expression = value?.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                    if (_expression?.Length == 0)
                    {
                        _expression = null;
                    }
                }
            }

            [JsonIgnore]
            public bool Success => Severity == IssueSeverity.Information || Severity == IssueSeverity.Warning;
        }

        [JsonConverter(typeof(JsonConverterFhirEnum<IssueSeverity>))]
        public enum IssueSeverity
        {
            Fatal,
            Error,
            Warning,
            Information
        }

        [JsonConverter(typeof(JsonConverterFhirEnum<IssueType>))]
        public enum IssueType
        {
            [Description("Invalid Content")] Invalid,
            [Description("Structural Issue")] Structure,
            [Description("Required element missing")] Required,
            [Description("Element value invalid")] Value,
            [Description("Validation rule failed")] Invariant,
            [Description("Security Problem")] Security,
            [Description("Login Required")] Login,
            [Description("Unknown User")] Unknown,
            [Description("Session Expired")] Expired,
            [Description("Forbidden")] Forbidden,
            [Description("Information  Suppressed")] Suppressed,
            [Description("Processing Failure")] Processing,
            [Description("Content not supported")] NotSupported,
            [Description("Duplicate")] Duplicate,
            [Description("Multiple Matches")] MultipleMatches,
            [Description("Not Found")] NotFound,
            [Description("Deleted")] Deleted,
            [Description("Content Too Long")] TooLong,
            [Description("Invalid Code")] CodeInvalid,
            [Description("Unacceptable Extension")] Extension,
            [Description("Operation Too Costly")] TooCostly,
            [Description("Business Rule Violation")] BusinessRule,
            [Description("Edit Version Conflict")] Conflict,
            [Description("Transient Issue")] Transient,
            [Description("Lock Error")] LockError,
            [Description("No Store Available")] NoStore,
            [Description("Exception")] Exception,
            [Description("Timeout")] Timeout,
            [Description("Incomplete Results")] Incomplete,
            [Description("Throttled")] Throttled,
            [Description("Informational Note")] Informational,
        }
    }
}
