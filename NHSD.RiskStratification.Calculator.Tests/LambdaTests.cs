using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.XRay.Recorder.Core;
using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Newtonsoft.Json;
using NHSD.RiskStratification.Calculator.FhirJson;
using NHSD.RiskStratification.Calculator.Infrastructure.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace NHSD.RiskStratification.Calculator.Tests
{
    using Xunit.Abstractions;

    [UseReporter(typeof(DiffReporter))]
    public abstract class LambdaTests : IDisposable
    {
        protected readonly ILogger Logger;

        protected abstract Func<APIGatewayProxyRequest, ILambdaContext, APIGatewayProxyResponse> LambdaFunction { get; }

        private readonly string[] _logPropertyKeys = {
            "correlationId",
            "requestId",
            "method",
            "url",
            "origin",
            "SourceContext",
            "applicationVersion",
            "algorithmVersion",
            "algorithmAssemblyName",
            "componentName",
            "environment",
            "riskAssessmentType"
        };

        protected LambdaTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .Destructure.With<FhirDestructuringPolicy>()
                .Destructure.With<QCovidDestructuringPolicy>()
                .WriteTo.TestCorrelator()
                .WriteTo.TestOutput(output)
                .CreateLogger();

            AWSXRayRecorder.Instance.BeginSegment("LambdaTest");
        }

        protected RiskAssessment ActAssertingRiskAssessment(Bundle observationBundle)
        {
            return ActAssertingResponse<RiskAssessment>(observationBundle, HttpStatusCode.OK);
        }

        protected OperationOutcome ActAssertingFailedOperationOutcome(Bundle observationBundle, HttpStatusCode expectedCode)
        {
            var operationOutcome = ActAssertingResponse<OperationOutcome>(observationBundle, expectedCode);
            operationOutcome.Success.Should().BeFalse();

            return operationOutcome;
        }

        protected TFhirResponse ActAssertingResponse<TFhirResponse>(Bundle observationBundle, HttpStatusCode expectedCode)
        {
            var result = Act(observationBundle);

            result.StatusCode.Should().Be((int)expectedCode);
            return JsonConvert.DeserializeObject<TFhirResponse>(result.Body);
        }

        protected APIGatewayProxyResponse Act(Bundle observationBundle)
        {
            return Act(JsonConvert.SerializeObject(observationBundle));
        }

        protected APIGatewayProxyResponse Act(string bodyContent)
        {
            var context = new TestLambdaContext();

            return LambdaFunction.Invoke(
                    new APIGatewayProxyRequest
                    {
                        Body = bodyContent,
                        Headers = new Dictionary<string, string>()
                    },
                    context);
        }

        protected void AssertInvalidErrorIssue(Calculator.FhirJson.OperationOutcome.IssueComponent issue)
        {
            issue.Severity.Should().Be(Calculator.FhirJson.OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Invalid);
        }

        protected void AssertIncompleteErrorIssue(OperationOutcome.IssueComponent issue)
        {
            issue.Severity.Should().Be(OperationOutcome.IssueSeverity.Error);
            issue.Code.Should().Be(OperationOutcome.IssueType.Incomplete);
        }

        protected void VerifyLogs(IList<LogEvent> logEvents)
        {
            logEvents.Should().HaveCountGreaterThan(0);

            // Known epoch to make the logs deterministic
            var time = new DateTimeOffset(2020, 3, 14, 9, 00, 00, TimeSpan.Zero);

            using (var log = new StringWriter())
            {
                var formatter = new CompactJsonFormatter();

                foreach (var logEvent in logEvents)
                {
                    var logEventProperties = logEvent.Properties.Select(r => new LogEventProperty(r.Key, r.Value));

                    var normalisedEvent = new LogEvent(time, logEvent.Level, logEvent.Exception, logEvent.MessageTemplate, logEventProperties);

                    VerifyLogEventContainsRequiredProperties(normalisedEvent);

                    formatter.Format(normalisedEvent, log);

                    time = time.AddSeconds(1);
                }

                Approvals.VerifyJson(log.ToString());
            }
        }

        protected void VerifyLogEventContainsRequiredProperties(LogEvent logEvent)
        {
            logEvent.Properties.Keys.Should().Contain(_logPropertyKeys);
        }

        public void Dispose()
        {
            AWSXRayRecorder.Instance.EndSegment();
        }
    }
}
