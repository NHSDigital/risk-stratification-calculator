using System.Reflection;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using NHSD.RiskStratification.Calculator.Algorithm;
using Serilog;
using Serilog.Core;
using Serilog.Core.Enrichers;
using Serilog.Formatting.Compact;

namespace NHSD.RiskStratification.Calculator.Infrastructure.Logging
{
    public static class LogManager
    {
        static LogManager()
        {
            Log.Logger = LoggingConfiguration()
                .CreateLogger();
        }

        public static LoggerConfiguration LoggingConfiguration()
        {
            return new LoggerConfiguration()
                .Destructure.With<FhirDestructuringPolicy>()
                .Destructure.With<QCovidDestructuringPolicy>()
                .WriteTo.Console(formatter: new CompactJsonFormatter());
        }

        public static ILogger GetLogger()
        {
            return Log.Logger;
        }

        public static void EnsureLogsAreFlushed()
        {
            Log.CloseAndFlush();
        }

        public static ILogger GetLoggerWithAlgorithmContext<T>(this ILogger log, AlgorithmAssembly algorithmAssembly, string riskAssessmentType)
        {
            return log.ForContext(AlgorithmEnrichments(algorithmAssembly, "calcengine", riskAssessmentType)).ForContext<T>();
        }

        public static ILogEventEnricher[] AlgorithmEnrichments(AlgorithmAssembly algorithmAssembly, string componentName, string riskAssessmentType)
        {
            var applicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return new[] {
                new PropertyEnricher("algorithmVersion", algorithmAssembly.Version),
                new PropertyEnricher("algorithmAssemblyName", algorithmAssembly.Name),
                new PropertyEnricher("applicationVersion", applicationVersion),
                new PropertyEnricher("componentName", componentName),
                new PropertyEnricher("environment", ApplicationConfiguration.EnvironmentName),
                new PropertyEnricher("riskAssessmentType", riskAssessmentType)
            };
        }

        public static ILogger GetLoggerWithRequestContext(this ILogger log, APIGatewayProxyRequest input, ILambdaContext context)
        {
            var correlationId = string.Empty;
            var userAgent = string.Empty;

            input.Headers?.TryGetValue("x-correlation-id", out correlationId);

            input.Headers?.TryGetValue("User-Agent", out userAgent);

            return log.ForContext(new[]
            {
                new PropertyEnricher("correlationId", correlationId),
                new PropertyEnricher("requestId", context.AwsRequestId),
                new PropertyEnricher("method", input.HttpMethod),
                new PropertyEnricher("url", input.Path),
                new PropertyEnricher("origin", userAgent)
            });
        }
    }
}
