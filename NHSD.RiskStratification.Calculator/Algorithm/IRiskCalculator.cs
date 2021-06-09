using System.Reflection;
using System.Threading.Tasks;

namespace NHSD.RiskStratification.Calculator.Algorithm
{
    public interface IRiskCalculator<TParameters, TResult>
    {
        TResult CalculateRisk(TParameters parameters);
    }
}
