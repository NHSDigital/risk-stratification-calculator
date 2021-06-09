using System;
using Microsoft.Extensions.Configuration;
using NHSD.RiskStratification.Calculator.Algorithm.QCovid;
using NHSD.RiskStratification.Calculator.Infrastructure.Configuration;

namespace NHSD.RiskStratification.Calculator.Infrastructure
{
    public static class ApplicationConfiguration
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                   .AddEnvironmentVariables("RISKSTRAT_")
                   .Build();

        public static string EnvironmentName { get; } = Environment.GetEnvironmentVariable("ENVIRONMENT");

        public static QCovidS3TownsendIndexConfiguration TownsendIndex { get; } = Configuration.GetSection("QCovidTownsendIndex").Get<QCovidS3TownsendIndexConfiguration>();

        //Needs to be brought in line with RISKSTRAT_ prefixed environment variables
        public static string QCovidParameterStore { get; } = Environment.GetEnvironmentVariable("QCOVID_PARAMETER_STORE");

        //default default value is only temporary.
        public static string QCovidAwsRegion { get; } = Environment.GetEnvironmentVariable("QCOVID_AWS_REGION") ?? "eu-west-2";

        public static QCovidSecrets QCovidSecrets { get; } =
            QCovidSecrets.ParameterStoreConfiguration(QCovidParameterStore, QCovidAwsRegion).Get<QCovidSecrets>();
    }
}
