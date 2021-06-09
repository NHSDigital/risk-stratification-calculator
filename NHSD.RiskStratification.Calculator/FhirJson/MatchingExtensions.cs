#nullable  enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.RiskStratification.Calculator.FhirJson
{
    public static class MatchingExtensions
    {
        public static bool Matches(this CodeableConcept source, CodeableConcept? pattern) => Matches(source, pattern, Match);

        public static bool Matches(this Coding source, Coding? pattern) => Matches(source, pattern, Match);

        public static bool Match(this CodeableConcept source, CodeableConcept pattern)
        {
            return Matches(source.Coding, pattern.Coding, Match) && Matches(source.Text, pattern.Text);
        }

        public static bool Match(this Coding source, Coding pattern)
        {
            return Matches(source.System, pattern.System) && Matches(source.Code, pattern.Code) && Matches(source.Display, pattern.Display);
        }

        public static bool Matches<T>(this T source, T? pattern, Func<T, T, bool> matcher) where T : struct
        {
            if (pattern is { } other)
            {
                return matcher(source, other);
            }

            return true;
        }

        private static bool Matches(string? source, string? pattern) => Matches(source, pattern, string.Equals);

        private static bool Matches<T>(T? source, T? pattern, Func<T, T, bool> matcher) where T : class
        {
            if (pattern is { } other)
            {
                return source != null && matcher(source, other);
            }

            return true;
        }

        private static bool Matches<T>(IEnumerable<T>? source, IEnumerable<T>? pattern, Func<T, T, bool> matcher) where T : struct
        {
            if (pattern == null)
            {
                return true;
            }

            return source != null && source.All(src => pattern.Any(patt => matcher(src, patt)));
        }
    }
}
