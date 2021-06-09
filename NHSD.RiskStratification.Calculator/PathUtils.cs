using System.Linq;

namespace NHSD.RiskStratification.Calculator
{
    public static class PathUtils
    {
        public static string CombineAwsPath(params string[] parts)
        {
            return string.Join('/', parts.Select(p => p.TrimEnd('/', ' ')).Where(p => !string.IsNullOrEmpty(p)));
        }
    }
}
