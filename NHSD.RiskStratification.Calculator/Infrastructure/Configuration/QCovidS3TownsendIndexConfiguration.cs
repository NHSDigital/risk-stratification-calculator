namespace NHSD.RiskStratification.Calculator.Infrastructure.Configuration
{
    public class QCovidS3TownsendIndexConfiguration
    {
        public string Region { get; set; }
        public string Bucket { get; set; }
        public string Path { get; set; } = string.Empty;
    }
}
