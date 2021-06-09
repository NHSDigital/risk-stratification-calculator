using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid
{
    public sealed class QCovidSecrets
    {
        public string QCovidLicenceKey { get; set; }

        public static IConfiguration ParameterStoreConfiguration(string qcovidParameterStore, string qcovidParameterStoreRegion)
        {
            var builder = new ConfigurationBuilder().AddEnvironmentVariables();

            if (!string.IsNullOrWhiteSpace(qcovidParameterStore))
            {
                var versionedParameterStore = PathUtils.CombineAwsPath(qcovidParameterStore, QCovidCalculator.AlgorithmAssembly.Version);

                builder.AddSystemsManager(versionedParameterStore, new AWSOptions { Region = RegionEndpoint.GetBySystemName(qcovidParameterStoreRegion) })
                    .Build();
            }

            return builder.Build();
        }
    }
}
