using System.IO;

namespace NHSD.RiskStratification.Calculator.Algorithm.QCovid.TownsendIndex
{
    public interface ITownsendIndexReader
    {
        (Stream stream, long length) GetIndexStream(byte indexKey);
    }
}
