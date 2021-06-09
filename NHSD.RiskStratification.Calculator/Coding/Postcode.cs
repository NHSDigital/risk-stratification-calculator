using System;

namespace NHSD.RiskStratification.Calculator.Coding
{
    public class Postcode
    {
        // Valid formats taken from http://www.mrs.org.uk/pdf/postcodeformat.pdf
        private static readonly string[] ValidFormats =
        {
            "AN NAA",
            "ANN NAA",
            "AAN NAA",
            "AANN NAA",
            "ANA NAA",
            "AANA NAA"
        };

        private readonly string _postcode;

        private Postcode(string postcode)
        {
            this._postcode = postcode;
        }

        public string Area => _postcode.Substring(0, char.IsLetter(_postcode[1]) ? 2 : 1);

        public string District => _postcode[..^4].Substring(Area.Length);

        public char Sector => _postcode[^3];

        public string Unit => _postcode[^2..];

        public override string ToString() => _postcode;

        public static bool TryParse(string s, out Postcode postcode)
        {
            postcode = null;

            if (s is null)
            {
                return false;
            }

            const int maxLength = 8;
            Span<char> p = stackalloc char[maxLength];
            Span<char> pattern = stackalloc char[maxLength];
            int length = 0;

            for (int i = s.Length - 1; i >= 0; i--)
            {
                var c = s[i];

                if (char.IsWhiteSpace(c))
                {
                    continue;
                }

                length++;

                if (length > maxLength)
                {
                    return false;
                }

                if (char.IsDigit(c))
                {
                    p[^length] = c;
                    pattern[^length] = 'N';
                }
                else if (char.IsLetter(c))
                {
                    p[^length] = char.ToUpper(c);
                    pattern[^length] = 'A';
                }
                else
                {
                    return false;
                }

                if (length == 3)
                {
                    length++;
                    p[^length] = ' ';
                    pattern[^length] = ' ';
                }
            }

            p = p[^length..];
            pattern = pattern[^length..];

            foreach (var validFormat in ValidFormats)
            {
                if (pattern.SequenceEqual(validFormat))
                {
                    // If we haven't reformatted the use the existing string else create a new one.
                    s = p.SequenceEqual(s) ? s : new string(p);

                    postcode = new Postcode(s);
                    return true;
                }
            }

            return false;
        }
    }
}
