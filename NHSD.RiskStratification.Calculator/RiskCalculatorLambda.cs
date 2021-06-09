using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.XRay.Recorder.Core;
using NHSD.RiskStratification.Calculator.Algorithm;
using NHSD.RiskStratification.Calculator.Infrastructure.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using NHSD.RiskStratification.Calculator.FhirJson;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace NHSD.RiskStratification.Calculator
{
    public abstract class RiskCalculatorLambda<TParameters>
    {
        protected abstract IObservationParameterExtractor<TParameters> ParameterExtractor { get; }
        protected abstract RiskAssessmentCalculator<TParameters> RiskCalculator { get; }

        protected ILogger Logger { get; set; }

        public APIGatewayProxyResponse FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            SetLoggerWithRequestContext(input, context);

            try
            {
                var bundle = Trace("Parse FHIR Bundle", () => JsonConvert.DeserializeObject<Resource>(input.Body) as Bundle);

                var parameterExtraction = Trace("Extract parameter Observations", () => ObservationParameterExtractionContext<TParameters>.FromBundle(ParameterExtractor, bundle));

                if (parameterExtraction.Success)
                {
                    Logger.Information("Successfully extracted parameters {@data}", parameterExtraction.Parameters);

                    var riskAssessmentCalculation = Trace("Create RiskAssessment", () => RiskCalculator.CreateAssessment(parameterExtraction));

                    if (riskAssessmentCalculation.TryGetAssessment(out var riskAssessment, out var error))
                    {
                        // We do not want to log the observations as they can contain personally identifiable data so instead just log the predictions and issues. 
                        LoggerWithCalculationOutcomeContext(riskAssessmentCalculation.Issues).Information("Created a risk assessment result {@data}", new { riskAssessmentCalculation.Predictions, riskAssessmentCalculation.Issues });

                        return Trace("CreateSuccessResponse", () => Response(HttpStatusCode.OK, riskAssessment));
                    }
                    else
                    {
                        LoggerWithCalculationOutcomeContext(riskAssessmentCalculation.Issues).Error("Failed to create a risk assessment result {@data}", new { riskAssessmentCalculation.Issues });

                        return Trace("CreateErrorResponse", () => Response(HttpStatusCode.InternalServerError, error));
                    }
                }

                var outcome = new OperationOutcome
                {
                    Meta = new Meta
                    {
                        VersionId = RiskCalculator.AlgorithmAssembly.FullName
                    },
                    Issue = parameterExtraction.Issues.Select(x => x.AsFhirElement()).ToList()
                };

                LoggerWithCalculationOutcomeContext(outcome.Issue).Error("Failed to extract parameters {@data}", outcome);

                return Trace("CreateErrorResponse", () => Response(HttpStatusCode.BadRequest, outcome));
            }
            catch (JsonException e)
            {
                var outcome = ExceptionOutcome(e, $"Invalid Json encountered: {e.Message}");
                LoggerWithCalculationOutcomeContext(outcome.Issue).Error(e, "Incoming request data was in an invalid format.");

                return Response(HttpStatusCode.BadRequest, outcome);
            }
            catch (Exception e)
            {
                var outcome = ExceptionOutcome(e, e.Message);
                LoggerWithCalculationOutcomeContext(outcome.Issue).Error(e, "An unexpected error occured.");

                return Response(HttpStatusCode.InternalServerError, outcome);
            }
            finally
            {
                LogManager.EnsureLogsAreFlushed();
            }
        }

        private OperationOutcome ExceptionOutcome(Exception e, string msg)
        {
            return new OperationOutcome
            {
                Issue =
                {
                    new OperationOutcome.IssueComponent
                    {
                         Severity = OperationOutcome.IssueSeverity.Error,
                         Code = OperationOutcome.IssueType.Invalid,
                         Diagnostics = $"Unable to create RiskAssessment: {msg}"
                    }
                }
            };
        }

        private APIGatewayProxyResponse Response(HttpStatusCode status, Resource content)
        {
            var response = new APIGatewayProxyResponse
            {
                IsBase64Encoded = false,
                StatusCode = (int)status,
                Body = JsonConvert.SerializeObject(content),
                Headers = new Dictionary<string, string>
                {
                    ["Content-Type"] = "application/fhir+json"
                }
            };

            return response;
        }

        private ILogger LoggerWithCalculationOutcomeContext(IReadOnlyCollection<OperationOutcome.IssueComponent> issues)
        {
            string status;

            if (issues.Count == 0)
            {
                status = "success";
            }
            else if (issues.Any(x => x.Success == false))
            {
                status = "fail";
            }
            else
            {
                status = "warn";
            }

            return Logger.ForContext("calculation", status);
        }

        private void SetLoggerWithRequestContext(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Logger = Logger.GetLoggerWithRequestContext(input, context);
        }

        private static TFunc Trace<TFunc>(string name, Func<TFunc> method)
        {
            return AWSXRayRecorder.Instance.TraceMethod(name, method);
        }
    }
}
