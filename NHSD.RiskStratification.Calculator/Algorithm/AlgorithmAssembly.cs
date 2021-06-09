using System;
using System.Reflection;

namespace NHSD.RiskStratification.Calculator.Algorithm
{
    public class AlgorithmAssembly
    {
        private readonly AssemblyName _assemblyName;

        public string Name => _assemblyName.Name;

        public string Version => _assemblyName.Version.ToString();

        public string FullName => $"{Name} {Version}";

        public AlgorithmAssembly(Type type)
        {
            _assemblyName = type.Assembly.GetName();
        }
    }
}
