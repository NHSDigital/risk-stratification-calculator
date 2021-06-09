using QCovid.RiskCalculator.BodyMassIndex;
using QCovid.RiskCalculator.Exceptions;
using QCovid.RiskCalculator.Risk.Input;
using QCovid.RiskCalculator.Townsend;
using Serilog.Core;
using Serilog.Events;

namespace NHSD.RiskStratification.Calculator.Infrastructure.Logging
{
    public class QCovidDestructuringPolicy : IDestructuringPolicy
    {
        public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue result)
        {
            LogEventPropertyValue Destructure(object obj) => propertyValueFactory.CreatePropertyValue(obj, true);

            result = value switch
            {
                Age age => new ScalarValue(age.Years),
                Bmi { ErrorCode: null } bmi => new ScalarValue(bmi.BodyMassIndex),
                Bmi { ErrorCode: { } error } => Destructure(error),
                EncryptedTownsendScore { ErrorCode: null, Value: { } townsend } => new ScalarValue(townsend),
                EncryptedTownsendScore { ErrorCode: { } error } => Destructure(error),
                IInputOption option => new ScalarValue(option.DisplayName),
                QCovidErrorCode error => Destructure(new { error.Code, error.Message }),
                _ => null
            };

            return result != null;
        }
    }
}
