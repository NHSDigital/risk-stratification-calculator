namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex
{
    public static class ByteExtensions
    {
        public static string ByteString(this byte byteValue)
        {
            return byteValue.ToString("X2");
        }
    }
}
